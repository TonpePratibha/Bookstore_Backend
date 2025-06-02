using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class CartRepository:ICartRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<CartRepository> _logger;

        public CartRepository(ApplicationDbContext context, JwtHelper jwtHelper, ILogger<CartRepository> logger)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        

        public CartModel AddToCart(string token, int bookId)
        {
            try
            {
                _logger.LogInformation("AddToCart called for BookId: {BookId}", bookId);
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

                // Check if the cart already contains this book for the user
                var existingCartItem = _context.Cart.FirstOrDefault(c => c.PurchasedBy == userId && c.BookId == bookId && !c.IsPurchased);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += 1;
                    existingCartItem.Price = (decimal)book.Price * existingCartItem.Quantity;
                    _context.Cart.Update(existingCartItem);
                    _logger.LogInformation("Updated quantity for existing cart item.");
                }
                else
                {
                    var newCart = new Cart
                    {
                        PurchasedBy = userId,
                        BookId = bookId,
                        Quantity = 1,
                        Price = (decimal)book.Price,
                        IsPurchased = false
                    };
                    _context.Cart.Add(newCart);
                    _logger.LogInformation("Added new book to cart.");
                }

                _context.SaveChanges();

                return new CartModel
                {
                    PurchasedBy = userId,
                    BookId = bookId,
                    Quantity = existingCartItem != null ? existingCartItem.Quantity : 1,
                    Price = existingCartItem != null ? existingCartItem.Price : (decimal)book.Price,
                    IsPurchased = false,
                    UserFirstName = user.FirstName,
                    UserLastName = user.LastName,
                    UserEmail = user.Email
                   
                };
            }
            catch
            {
                _logger.LogError( "Error occurred while adding to cart.");
                return null;
            }
        }








        public CartModel UpdateCartQuantity(string token, int bookId, int newQuantity)
        {
            try
            {
                _logger.LogInformation("UpdateCartQuantity called for BookId: {BookId} with new quantity: {Quantity}", bookId, newQuantity);
                var userId = _jwtHelper.ExtractUserIdFromJwt(token);
                var role = _jwtHelper.ExtractRoleFromJwt(token);

                if (role.ToLower() != "user") return null;

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);

                var cartItem = _context.Cart.FirstOrDefault(c =>
                    c.PurchasedBy == userId && c.BookId == bookId && !c.IsPurchased);

                if (cartItem == null)
                    return null;

                // ❗Prevent quantity below 1
                if (newQuantity < 1)
                {
                    _logger.LogWarning("Attempt to set quantity below 1 for BookId: {BookId}", bookId);
                    // Optionally: return current cart state instead of null
                    return new CartModel
                    {
                        PurchasedBy = userId,
                        UserFirstName = user.FirstName,
                        UserLastName = user.LastName,
                        UserEmail = user.Email,
                        BookId = bookId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price
                    };
                }

                var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
                if (book == null)
                    return null;

                cartItem.Quantity = newQuantity;
                cartItem.Price = (decimal)(book.Price * newQuantity);

                _context.SaveChanges();

                return new CartModel
                {
                    PurchasedBy = userId,
                    UserFirstName = user.FirstName,
                    UserLastName = user.LastName,
                    UserEmail = user.Email,
                    BookId = bookId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating cart quantity.");
                return null;
            }
        }


        public string DeleteFromCartIfQuantityZero(string token, int bookId)
        {
            try
            {
                _logger.LogInformation("DeleteFromCartIfQuantityZero called for BookId: {BookId}", bookId);
                var role = _jwtHelper.ExtractRoleFromJwt(token);
                var userId = _jwtHelper.ExtractUserIdFromJwt(token);

                if (string.IsNullOrEmpty(role) || role.ToLower() != "user")
                    return "Unauthorized. Only users can delete from cart.";

                var cartItem = _context.Cart.FirstOrDefault(c => c.BookId == bookId && c.PurchasedBy == userId);

                if (cartItem == null)
                    return "Cart item not found.";

            

                _context.Cart.Remove(cartItem);
                _context.SaveChanges();
                _logger.LogInformation("Cart item deleted successfully.");
                return "Cart item deleted successfully.";
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting cart item.");
                return $"An error occurred while deleting the cart item: {ex.Message}";
            }
        }




       
      



        public CartResponseModel GetCartDetails(string token)
        {
            try
            {
                _logger.LogInformation("GetCartDetails called.");
                int userId = _jwtHelper.ExtractUserIdFromJwt(token);
                string role = _jwtHelper.ExtractRoleFromJwt(token);

                if (role.ToLower() != "user")
                    return new CartResponseModel { IsSuccess = false, Message = "Only users are allowed to access the cart." };

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                    return new CartResponseModel { IsSuccess = false, Message = "User not found." };

                var cartItems = _context.Cart
                    .Where(c => c.PurchasedBy == userId && !c.IsPurchased)
                    .Include(c => c.Book)
                    .ToList();

                if (!cartItems.Any())
                    return new CartResponseModel { IsSuccess = false, Message = "Cart is empty or not found." };

                var cartList = cartItems.Select(c => new CartItemModel
                {
                    BookId = c.BookId,
                    BookName = c.Book.BookName,
                    Author = c.Book.Author,
                    Quantity = c.Quantity,
                    Price = c.Price,
                    BookImage = c.Book.BookImage
                }).ToList();


                _logger.LogInformation("Cart fetched successfully for user: {UserId}", userId);

                return new CartResponseModel
                {
                    IsSuccess = true,
                    Message = "Cart fetched successfully.",
                    Data = new CartSummeryModel
                    {
                        Items = cartList,
                        TotalQuantity = cartList.Sum(x => x.Quantity),
                        TotalCost = cartList.Sum(x => x.Price),
                        User = new UserDetailsModel
                        {
                            UserId = user.Id,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName= user.LastName
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting cart details.");
                return new CartResponseModel { IsSuccess = false, Message = $"Internal error: {ex.Message}" };
            }
        }

    }
}
