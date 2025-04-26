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
    public class WishListService:IWishListService
    {
        public readonly IWishListRepository _wishListRepository;

        public WishListService( IWishListRepository wishListRepository) { 
          _wishListRepository = wishListRepository;
        }
        public WishListModel AddToWishList(string token, int bookId) { 
        return _wishListRepository.AddToWishList(token, bookId);   
        }

        public string RemoveFromWishlist(string token, int bookId) {

            return _wishListRepository.RemoveFromWishlist(token, bookId);
        }

        public WishListResponseModel GetWishListDetails(string token) {
            return _wishListRepository.GetWishListDetails(token);
        }
    }
}
