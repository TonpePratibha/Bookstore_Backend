using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IWishListRepository
    {
        public WishListModel AddToWishList(string token,int bookId);

        public string RemoveFromWishlist(string token, int bookId);

        public WishListResponseModel GetWishListDetails(string token);
    }
}
