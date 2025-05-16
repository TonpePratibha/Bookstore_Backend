using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
 public class OrderResponseModel
    {
       
            public int OrderId { get; set; }
            public int OrderedBy { get; set; }
            public string UserFirstName { get; set; }
            public string UserLastName { get; set; }
            public string UserEmail { get; set; }
            public int BookId { get; set; }
            public string BookName { get; set; }
            public string BookImage { get; set; }
            public string Author { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public DateTime OrderDate { get; set; }
        }

    }

