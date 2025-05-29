using BuisnessLayer.Interface;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.Modal;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Service
{
    public class BookService : IBookService
    {

        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public string LoadBooksFromCsv(string token)
        {
            return _bookRepository.LoadBooksFromCsv(token);
        }



        public IEnumerable<Book> GetAllBooks()
        {
            return _bookRepository.GetAllBooks();
        }

        public PaginatedBooksResult GetAllBooksWithPage(int page) { 
        return _bookRepository.GetAllBooksWithPage(page);
        }
        public IEnumerable<Book> GetAllRecentBooks() { 
        return _bookRepository.GetAllRecentBooks();
        }

        public Book GetBookById(int id)
        {

            return _bookRepository.GetBookById(id);


        }

        public IEnumerable<Book> SearchBooks(string search) {
            return _bookRepository.SearchBooks(search);
        }


/*
        public IEnumerable<Book> SearchBooksByAuthor(string author)
        {


            return _bookRepository.SearchBooksByAuthor(author);

        }

        public IEnumerable<Book> SearchBooksByTitle(string title)
        {
            
            
                return _bookRepository.SearchBooksByTitle(title);
            }
*/

        public IEnumerable<Book> GetBooksSortedByPriceAsc()
        {
           
                return _bookRepository.GetBooksSortedByPriceAsc();
           
          
        }

        public IEnumerable<Book> GetBooksSortedByPriceDesc()
        {
           
                return _bookRepository.GetBooksSortedByPriceDesc();
            
        }

       
       
        public Book AddBook(BookModel model,  string token)
        {
            return _bookRepository.AddBook(model, token);
        }

        public Book UpdateBook(int id, BookModel model, string token)
        {
            return _bookRepository.UpdateBook(id, model, token);
        }

        public string DeleteBook(int id, string token)
        {
            return _bookRepository.DeleteBook(id, token);
        }



    }
}
