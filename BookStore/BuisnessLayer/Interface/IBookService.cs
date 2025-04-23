using DataAccessLayer.Entity;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Interface
{
  public interface IBookService
    {
        public string LoadBooksFromCsv(string token);
        public Book AddBook(BookModel model, string token);
        public Book UpdateBook(int id, BookModel model, string token);
        public string DeleteBook(int id, string token);
        public List<Book> GetAllBooks();
        public List<Book> GetAllBooksWithPage(int page);
        public Book GetMostRecentBook();
        public Book GetBookById(int id);
        public IEnumerable<Book> SearchBooksByAuthor(string author);
        IEnumerable<Book> SearchBooksByTitle(string title);
        public IEnumerable<Book> GetBooksSortedByPriceAsc();
        public IEnumerable<Book> GetBooksSortedByPriceDesc();
    }
}
