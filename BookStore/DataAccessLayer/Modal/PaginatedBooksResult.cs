using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
   public class PaginatedBooksResult
    {
        public List<Book> Books { get; set; }
        public int TotalCount { get; set; }
    }
}
