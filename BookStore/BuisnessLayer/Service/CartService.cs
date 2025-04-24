using BuisnessLayer.Interface;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Service
{
 public class CartService:ICartService
    {

        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository) { 
          _cartRepository = cartRepository;
        }

        public string AddToCart(string token, int bookId)
        { 
        
        return _cartRepository.AddToCart(token, bookId);

                }
    }
}
