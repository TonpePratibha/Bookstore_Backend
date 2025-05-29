using DataAccessLayer.Entity;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IBookRepository
    {
        public string LoadBooksFromCsv(string token);
        public Book AddBook(BookModel model, string token);
        public Book UpdateBook(int id, BookModel model, string token);
        public string DeleteBook(int id, string token);
      
        public IEnumerable<Book> GetAllBooks();


        public PaginatedBooksResult GetAllBooksWithPage(int page);
        public IEnumerable<Book> GetAllRecentBooks();
        public Book GetBookById(int id);
        public IEnumerable<Book> SearchBooks(string search);
       
        public IEnumerable<Book> GetBooksSortedByPriceAsc();
        public IEnumerable<Book> GetBooksSortedByPriceDesc();
    }
}
