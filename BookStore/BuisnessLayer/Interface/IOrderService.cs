using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Interface
{
   public interface IOrderService
    {
        public OrderResponse PlaceOrder(string token);
        public List<OrderItemresponse> GetOrdersByUser(string token);
    }
}
