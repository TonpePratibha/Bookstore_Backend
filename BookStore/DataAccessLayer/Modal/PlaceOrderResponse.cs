using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
 public class PlaceOrderResponse
    {
        public decimal TotalPrice { get; set; }
        public List<OrderItemresponse> Items { get; set; }
    }
}
