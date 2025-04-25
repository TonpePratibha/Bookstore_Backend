using DataAccessLayer.Entity;
using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
   public interface ICartRepository
    {
        public CartModel AddToCart(string token, int bookId);
        public CartModel UpdateCartQuantity(string token, int bookId, int newQuantity);
        public string DeleteFromCartIfQuantityZero(string token, int bookId);
        //public CartSummeryModel GetCartDetails(string token);

        public CartResponseModel GetCartDetails(string token);
    }
}
