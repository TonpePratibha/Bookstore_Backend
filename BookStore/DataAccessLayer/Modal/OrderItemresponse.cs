using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
    public class OrderItemresponse
    {

        public string BookName { get; set; }
        public string Author { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerItem { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
