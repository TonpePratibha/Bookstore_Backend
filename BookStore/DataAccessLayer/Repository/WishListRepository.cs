using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class WishListRepository : IWishListRepository
    {


        public readonly JwtHelper _jwtHelper;
        public ApplicationDbContext _context;
        public WishListRepository(JwtHelper jwtHelper, ApplicationDbContext context)
        {


            _jwtHelper = jwtHelper;
            _context = context;


        }
      

        public WishListModel AddToWishList(string token, int bookId)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return null;

                var role = _jwtHelper.ExtractRoleFromJwt(token);
                var userId = _jwtHelper.ExtractUserIdFromJwt(token);

                if (role.ToLower() != "user") return null;

              
                var existing = _context.Wishlist
                    .FirstOrDefault(w => w.AddedBy == userId && w.BookId == bookId);

                if (existing != null)
                {
                    throw new Exception("Book already present in wishlist.");
                }

               
                _context.Database.ExecuteSqlRaw("EXEC AddToWishlist @UserId = {0}, @BookId = {1}", userId, bookId);

               
                var wishlist = _context.Wishlist
                    .Include(w => w.Book)
                    .Include(w => w.User)
                    .FirstOrDefault(w => w.AddedBy == userId && w.BookId == bookId);

                if (wishlist == null)
                    return null;

                return new WishListModel
                {
                    AddedBy = userId,
                    BookId = bookId,
                    BookName = wishlist.Book.BookName,
                    Author = wishlist.Book.Author,
                    Description = wishlist.Book.Description,
                    Price = wishlist.Book.Price,
                    DiscountPrice = wishlist.Book.DiscountPrice,
                    Quantity = wishlist.Book.Quantity,
                    BookImage = wishlist.Book.BookImage,
                    UserFirstName = wishlist.User.FirstName,
                    UserLastName = wishlist.User.LastName,
                    UserEmail = wishlist.User.Email
                };
            }
            catch (Exception ex)
            {
                // 👇 Throw the specific error upward so controller can handle it
                throw new Exception(ex.Message);
            }
        }



        public string RemoveFromWishlist(string token, int bookId){
            try
            {
                var role = _jwtHelper.ExtractRoleFromJwt(token);
                var userId = _jwtHelper.ExtractUserIdFromJwt(token);

                if (string.IsNullOrEmpty(role) || role.ToLower() != "user")
                    return "Unauthorized. Only users can delete from wishlist.";

                //   var wishlistItem = _context.Wishlist.FirstOrDefault(c => c.BookId == bookId && c.AddedBy == userId);  // using linq
              var wishlistItem=  _context.Database.ExecuteSqlRaw("EXEC deletewishlistbyuserid @userid = {0}, @bookid = {1}", userId, bookId);  //using stored procoedure

                if (wishlistItem == null)
                    return "wishlist item not found.";

                
              //  _context.Wishlist.Remove(wishlistItem);
              //  _context.SaveChanges();

                return "wishlist item deleted successfully.";
            }

            catch (Exception ex)
            {

                return $"An error occurred while deleting the wishlist item: {ex.Message}";
            }



        }


        public WishListResponseModel GetWishListDetails(string token)
        {
            try
            {
                int userId = _jwtHelper.ExtractUserIdFromJwt(token);
                string role = _jwtHelper.ExtractRoleFromJwt(token);

                if (role.ToLower() != "user")
                    return new WishListResponseModel { IsSuccess = false, Message = "Only users are allowed to access the wishlist." };

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                    return new WishListResponseModel { IsSuccess = false, Message = "User not found." };
                /*
                                var wishlistItems = _context.Wishlist
                                    .Where(c => c.AddedBy == userId )
                                    .Include(c => c.Book)
                                    .ToList(); 
                */   //using linque query
                 
               var wishlistItems= _context.WishlistBooks.FromSqlRaw("EXEC getwishlistbyuserid @userid = {0}", userId).ToList();

          //done using stored procedure
                if (wishlistItems == null || !wishlistItems.Any())
                    return new WishListResponseModel { IsSuccess = false, Message = "wishlist is empty or not found." };

             /*  
                var wishlistList = wishlistItems.Select(c => new WishlListItemModel    //while using linq query
                {
                    BookId = c.BookId,
                    BookName = c.Book.BookName,
                    Author = c.Book.Author,                 
                    Description = c.Book.Description,
                    Price = c.Book.Price,
                    DiscountPrice = c.Book.DiscountPrice,
                    Quantity = c.Book.Quantity,
                    BookImage = c.Book.BookImage


                }).ToList();
                */

                var wishlistList = wishlistItems.Select(c => new WishlListItemModel
                {
                    BookId = c.BookId,
                    BookName = c.BookName,
                    Author = c.Author,
                    Description = c.Description,
                    Price = c.Price,
                    DiscountPrice = c.DiscountPrice,
                    Quantity = c.Quantity,
                    BookImage = c.BookImage
                }).ToList();

                return new WishListResponseModel
                {
                    IsSuccess = true,
                    Message = "wishlist fetched successfully.",
                    Data = new WishListSummeryModel
                    {
                        Items = wishlistList,
                      
                        User = new UserDetailsModel
                        {
                            UserId = user.Id,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                return new WishListResponseModel { IsSuccess = false, Message = $"Internal error: {ex.Message}" };
            }
        }

    }



}

    
    
