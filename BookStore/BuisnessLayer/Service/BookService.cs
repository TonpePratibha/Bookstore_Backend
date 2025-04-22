using BuisnessLayer.Interface;
using DataAccessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Service
{
   public class BookService:IBookService
    {

        private readonly IBookRepository _repository;

        public BookService(IBookRepository repository)
        {
            _repository = repository;
        }

        public string LoadBooksFromCsv()
        {
            return _repository.LoadBooksFromCsv();
        }
    }
}
