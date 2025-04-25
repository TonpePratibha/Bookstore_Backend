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

        public CartModel AddToCart(string token, int bookId)
        { 
        
        return _cartRepository.AddToCart(token, bookId);

                }


        public CartModel UpdateCartQuantity(string token, int bookId, int newQuantity) { 
        
        return _cartRepository.UpdateCartQuantity(token, bookId, newQuantity);
        }
        public string DeleteFromCartIfQuantityZero(string token, int bookId) {


            return _cartRepository.DeleteFromCartIfQuantityZero(token, bookId);
        }
        /*
                public CartSummeryModel GetCartDetails(string token) { 

                return _cartRepository.GetCartDetails(token);

                }
                */

        public CartResponseModel GetCartDetails(string token) { 
        return _cartRepository.GetCartDetails(token);

        }
    }
}
