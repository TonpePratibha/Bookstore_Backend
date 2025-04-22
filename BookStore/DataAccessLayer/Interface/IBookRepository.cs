using DataAccessLayer.Entity;
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
        public List<Book> GetAllBooks();
        public Book GetBookById(int id);
        public IEnumerable<Book> SearchBooksByAuthor(string author);
        IEnumerable<Book> SearchBooksByTitle(string title);
        public IEnumerable<Book> GetBooksSortedByPriceAsc();
        public IEnumerable<Book> GetBooksSortedByPriceDesc();
    }
}
