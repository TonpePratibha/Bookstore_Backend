using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Interface
{
   public interface IWishListService
    {
        public WishListModel AddToWishList(string token, int bookId);
        public string RemoveFromWishlist(string token, int bookId);
        public WishListResponseModel GetWishListDetails(string token);
    }
}
