﻿using DataAccessLayer.Entity;
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
        public IEnumerable<Book> GetAllBooks();
        public PaginatedBooksResult GetAllBooksWithPage(int page);
        public IEnumerable<Book> GetAllRecentBooks();
        public Book GetBookById(int id);
        public IEnumerable<Book> SearchBooks(string search);
      //  public IEnumerable<Book> SearchBooksByAuthor(string author);
       // IEnumerable<Book> SearchBooksByTitle(string title);
        public IEnumerable<Book> GetBooksSortedByPriceAsc();
        public IEnumerable<Book> GetBooksSortedByPriceDesc();
    }
}
