using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IOrderRepository
    {
        public OrderResponse PlaceOrder(string token);
        public List<OrderItemresponse> GetOrdersByUser(string token);
    }
}
