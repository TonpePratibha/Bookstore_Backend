using CsvHelper;
using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class BookRepository:IBookRepository

    {

        private readonly  ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration; 

        public BookRepository(ApplicationDbContext context,JwtHelper jwtHelper,IConfiguration configuration)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _configuration = configuration;

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

            public List<Book>GetAllBooks()
          {
            try
            {
                return _context.Books.ToList();
            }
            catch (Exception ex)
            {
                return new List<Book>();
            }
        }

        public Book GetBookById(int id)
{
    try
    {
        var book = _context.Books.FirstOrDefault(b => b.Id == id);
        if (book == null)
            throw new KeyNotFoundException($"Book with ID {id} not found.");
        return book;
    }
    catch (Exception ex)
    {
        throw new Exception("Error fetching book by ID.", ex);
    }
}


        public IEnumerable<Book> SearchBooksByAuthor(string author)
        {
            try
            {
                return _context.Books
                               .Where(b => b.Author.ToLower().Contains(author.ToLower()))
                               .ToList();
            }
            catch (Exception ex)
            {
              
                throw new Exception("Error fetching books by author from database.", ex);
            }
        }

        public IEnumerable<Book> SearchBooksByTitle(string title)
        {
            try
            {
                return _context.Books
                               .Where(b => b.BookName.ToLower().Contains(title.ToLower()))
                               .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching books by title from database.", ex);
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
