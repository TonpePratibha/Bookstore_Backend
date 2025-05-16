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
        public WishListModel AddToWishList(string token, int bookId) {


            try
            {
                if (string.IsNullOrEmpty(token))
                    return null;

                var role = _jwtHelper.ExtractRoleFromJwt(token);
                var userId = _jwtHelper.ExtractUserIdFromJwt(token);

                if (role.ToLower() != "user")
                    return null;

                var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
                if (book == null)
                    return null;

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                    return null;

                var isInCart = _context.Cart.Any(c => c.PurchasedBy == userId && c.BookId == bookId);
                if (isInCart)
                {
                    // ❌ If the book is in the cart, don't add it to wishlist
                    return null;
                }

                // Check if the cart already contains this book for the user
                var existingItem = _context.Wishlist.FirstOrDefault(c => c.AddedBy == userId && c.BookId == bookId);

                if (existingItem != null)
                {
                    return null;
                }
                else
                {
                    var newwishlist = new WishList
                    {
                        AddedBy = userId,
                        BookId = bookId,


                    };
                    _context.Wishlist.Add(newwishlist);
                }

                _context.SaveChanges();

                return new WishListModel
                {
                    AddedBy = userId,
                    BookId = bookId,
                    BookName = book.BookName,
                    Author = book.Author,
                    Description = book.Description,
                    Price = book.Price,
                    DiscountPrice = book.DiscountPrice,
                    Quantity = book.Quantity,
                    BookImage = book.BookImage,

                    UserFirstName = user.FirstName,
                    UserLastName = user.LastName,
                    UserEmail = user.Email

                };
            }
            catch
            {
                return null;
            }
        }



        public string RemoveFromWishlist(string token, int bookId){
            try
            {
                var role = _jwtHelper.ExtractRoleFromJwt(token);
                var userId = _jwtHelper.ExtractUserIdFromJwt(token);

                if (string.IsNullOrEmpty(role) || role.ToLower() != "user")
                    return "Unauthorized. Only users can delete from wishlist.";

                var wishlistItem = _context.Wishlist.FirstOrDefault(c => c.BookId == bookId && c.AddedBy == userId);

                if (wishlistItem == null)
                    return "wishlist item not found.";

                
                _context.Wishlist.Remove(wishlistItem);
                _context.SaveChanges();

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

                var wishlistItems = _context.Wishlist
                    .Where(c => c.AddedBy == userId )
                    .Include(c => c.Book)
                    .ToList();

                if (!wishlistItems.Any())
                    return new WishListResponseModel { IsSuccess = false, Message = "wishlist is empty or not found." };

                var wishlistList = wishlistItems.Select(c => new WishlListItemModel
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

    
    
