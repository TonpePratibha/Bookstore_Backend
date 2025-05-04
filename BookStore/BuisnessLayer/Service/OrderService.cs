using BuisnessLayer.Interface;
using DataAccessLayer.Interface;
using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Service
{
  public class OrderService:IOrderService
    {
        public readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository) {
            _orderRepository = orderRepository;
        }
        public OrderResponse PlaceOrder(string token) { 
            return _orderRepository.PlaceOrder(token);
        }

        public List<OrderItemresponse> GetOrdersByUser(string token)
        {
            return _orderRepository.GetOrdersByUser(token);
        }


    }
}
