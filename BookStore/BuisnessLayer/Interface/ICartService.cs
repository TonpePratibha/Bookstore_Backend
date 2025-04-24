using DataAccessLayer.Entity;
using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Interface
{
   public interface ICartService
    {
        public string AddToCart(string token, int bookId);
        public string UpdateCartQuantity(string token, int bookId, int newQuantity);
        public string DeleteFromCartIfQuantityZero(string token, int bookId);
      //  public CartSummeryModel GetCartDetails(string token);
        public CartResponseModel GetCartDetails(string token);


    }
}
