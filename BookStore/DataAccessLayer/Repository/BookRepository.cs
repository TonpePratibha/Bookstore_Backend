using CsvHelper;
using DataAccessLayer.CustomException;
using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class BookRepository:IBookRepository

    {

        private readonly  ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;

        public BookRepository(ApplicationDbContext context,JwtHelper jwtHelper,IConfiguration configuration, IDistributedCache cache)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _configuration = configuration;
            _cache = cache;

        }


        public string LoadBooksFromCsv(string token)
        {
            var role = _jwtHelper.ExtractRoleFromJwt(token);
            if (role != "Admin")
            {
                return "Unauthorized: Only admin can load books.";
            }

            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string relativePath = @"..\..\..\..\DataAccessLayer\Csv\books.csv";
                string fullPath = Path.GetFullPath(Path.Combine(basePath, relativePath));

                var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                using (var reader = new StreamReader(fullPath))
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<Book>().ToList();

                    foreach (var book in records)
                    {
                        book.Id = 0;
                        book.CreatedAt = DateTime.Now;
                        book.UpdatedAt = DateTime.Now;
                    }

                    _context.Books.AddRange(records);
                    _context.SaveChanges();

                    return "Books loaded successfully from CSV.";
                }
            }
            catch (Exception ex)
            {
                return $"Error while loading books: {ex.Message}";
            }
        }


        public Book AddBook(BookModel model, string token)
        {
            try
            {
                var role = _jwtHelper.ExtractRoleFromJwt(token);
                var adminUserId = _jwtHelper.ExtractUserIdFromJwt(token);

                if (role != "Admin")
                    throw new UnauthorizedAccessException("Only admin can add books.");

                var book = new Book
                {
                    BookName = model.BookName,
                    Author = model.Author,
                    Description = model.Description,
                    Price = model.Price,
                    DiscountPrice = model.DiscountPrice,
                    Quantity = model.Quantity,
                    BookImage = model.BookImage,
                    AdminUserId = adminUserId.ToString(),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Books.Add(book);
                _context.SaveChanges();


                _cache.Remove("books:all");
                return book;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while adding book: {ex.Message}");
            }
        }

        public Book UpdateBook(int id, BookModel model, string token)
        {
            try
            {
                var role = _jwtHelper.ExtractRoleFromJwt(token);
                if (role != "Admin")
                    throw new UnauthorizedAccessException("Only admin can update books.");

                var book = _context.Books.FirstOrDefault(b => b.Id == id);
                if (book == null)
                    throw new Exception("Book not found.");

                book.BookName = model.BookName;
                book.Author = model.Author;
                book.Description = model.Description;
                book.Price = model.Price;
                book.DiscountPrice = model.DiscountPrice;
                book.Quantity = model.Quantity;
                book.BookImage = model.BookImage;
                book.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
                _cache.Remove("books:all");     // Remove book list cache
                _cache.Remove($"book:{id}");
                return book;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while updating book: {ex.Message}");
            }
        }
       



        public string DeleteBook(int id, string token)
        {
            try
            {
                var role = _jwtHelper.ExtractRoleFromJwt(token);
                if (role != "Admin")
                    throw new UnauthorizedAccessException("Only admin can delete books.");

                var book = _context.Books.FirstOrDefault(b => b.Id == id);
                if (book == null)
                    throw new Exception("Book not found.");

                _context.Books.Remove(book);
                _context.SaveChanges();

                
               _cache.Remove($"book:{id}");      // Remove individual book 
               _cache.Remove("books:all");       // Remove  books list cache

                return "Book deleted successfully";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while deleting book: {ex.Message}");
            }
        }




        public IEnumerable<Book> GetAllBooks()
        {
            const string cacheKey = "books:all";

            var cachedBooks = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cachedBooks))
            {
               Console.WriteLine("data from redis");
                return JsonSerializer.Deserialize<List<Book>>(cachedBooks);
            }

            var books = _context.Books.ToList();
            Console.WriteLine("data from db");

            if (books.Any())
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache duration
                };
                _cache.SetString(cacheKey, JsonSerializer.Serialize(books), cacheOptions);
            }

            return books;
        }

        public PaginatedBooksResult GetAllBooksWithPage(int page)
        {
            try
            {
                int pageSize = 8;
                int totalBooks = _context.Books.Count();
                int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

                if (page < 1 || page > totalPages)
                {
                    throw new PageNotFoundException($"Page {page} not found. Total pages: {totalPages}");
                }

                var books = _context.Books
                                    .OrderBy(b => b.Id)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

                return new PaginatedBooksResult
                {
                    Books = books,
                    TotalCount = totalBooks
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

      

        public IEnumerable<Book> GetAllRecentBooks()
        {
            try
            {
                const string cacheKey = "books:recent";

             
                var cachedBooks = _cache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cachedBooks))
                {
                    Console.WriteLine("Data from Redis cache (recent books)");
                    return JsonSerializer.Deserialize<List<Book>>(cachedBooks);
                }

               
                var recentBooks = _context.Books
                                          .OrderByDescending(b => b.CreatedAt)
                                          .ToList();

                Console.WriteLine("Data from DB (recent books)");

              
                if (recentBooks.Any())
                {
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    };
                    _cache.SetString(cacheKey, JsonSerializer.Serialize(recentBooks), cacheOptions);
                }

                return recentBooks;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving recent books: " + ex.Message, ex);
            }
        }



        public Book GetBookById(int id)
        {
            try
            {
                string cacheKey = $"book:{id}";

              
                var cachedBook = _cache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cachedBook))
                {
                    Console.WriteLine("Data from Redis cache.");
                    return JsonSerializer.Deserialize<Book>(cachedBook);
                }

             
                var book = _context.Books.FirstOrDefault(b => b.Id == id);
                if (book == null)
                    throw new KeyNotFoundException($"Book with ID {id} not found.");

                Console.WriteLine("Data from Database.");

             
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                _cache.SetString(cacheKey, JsonSerializer.Serialize(book), cacheOptions);

                return book;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching book by ID.", ex);
            }
        }


        public IEnumerable<Book> SearchBooks(string search)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(search))
                    return new List<Book>();

                string lowerSearch = search.ToLower();

                return _context.Books
                               .Where(b => b.Author.ToLower().Contains(lowerSearch) ||
                                           b.BookName.ToLower().Contains(lowerSearch))
                               .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching books from database.", ex);
            }
        }


  
          public IEnumerable<Book> GetBooksSortedByPriceAsc()
          {
              try
              {
                  return _context.Books.OrderBy(b => b.Price).ToList();
              }
              catch (Exception ex)
              {
                  throw new Exception("Repository error during ascending sort.", ex);
              }
          }
        
        public IEnumerable<Book> GetBooksSortedByPriceDesc()
        {
            try
            {
                return _context.Books.OrderByDescending(b => b.Price).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Repository error during descending sort.", ex);
            }
        }




    }
}
