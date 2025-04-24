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
    public class CartRepository:ICartRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public CartRepository(ApplicationDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public string AddToCart(string token, int bookId)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return "Token is missing";

                var role = _jwtHelper.ExtractRoleFromJwt(token);
                var userId = _jwtHelper.ExtractUserIdFromJwt(token);

                if (role.ToLower() != "user")
                    return "Only users can add to cart.";

                var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
                if (book == null)
                    return "Book not found.";

                var cart = new Cart
                {
                    PurchasedBy = userId,
                    BookId = bookId,
                    Quantity = 1,
                    Price = (decimal)book.Price,
                    IsPurchased = false
                };

                _context.Cart.Add(cart);
                _context.SaveChanges();

                return "Book added to cart.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


       


        public string UpdateCartQuantity(string token, int bookId, int newQuantity)
        {
            try
            {
                var userId = _jwtHelper.ExtractUserIdFromJwt(token);
                var role = _jwtHelper.ExtractRoleFromJwt(token);

                if (role.ToLower() != "user") return "Only users can update cart.";

                var cartItem = _context.Cart.FirstOrDefault(c =>
                    c.PurchasedBy == userId && c.BookId == bookId && !c.IsPurchased);

                if (cartItem == null)
                    return "Cart item not found.";

                if (newQuantity <= 0)
                {
                    _context.Cart.Remove(cartItem);
                    _context.SaveChanges();
                    return "Item removed from cart as quantity was 0.";
                }


                var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
                if (book == null)
                    return "Book not found.";

                cartItem.Quantity = newQuantity;
                cartItem.Price = (decimal)(book.Price * newQuantity);

                _context.SaveChanges();

                return "Cart updated successfully.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


        public string DeleteFromCartIfQuantityZero(string token, int bookId)
        {
            try
            {
                var role = _jwtHelper.ExtractRoleFromJwt(token);
                var userId = _jwtHelper.ExtractUserIdFromJwt(token);

                if (string.IsNullOrEmpty(role) || role.ToLower() != "user")
                    return "Unauthorized. Only users can delete from cart.";

                var cartItem = _context.Cart.FirstOrDefault(c => c.BookId == bookId && c.PurchasedBy == userId);

                if (cartItem == null)
                    return "Cart item not found.";

                if (cartItem.Quantity != 0)
                    return "Item quantity is not zero. Cannot delete.";

                _context.Cart.Remove(cartItem);
                _context.SaveChanges();

                return "Cart item deleted successfully.";
            }
            
            catch (Exception ex)
            {
                // Log exception if needed
                return $"An error occurred while deleting the cart item: {ex.Message}";
            }
        }
        /*
        public CartSummeryModel GetCartDetails(string token)
        {
            try
            {
                int userId = _jwtHelper.ExtractUserIdFromJwt(token);
                string role = _jwtHelper.ExtractRoleFromJwt(token);

                if (role.ToLower() != "user")
                    return null;

                var cartItems = _context.Cart
                    .Where(c => c.PurchasedBy == userId && !c.IsPurchased)
                    .Include(c => c.Book)
                    .ToList();

                if (!cartItems.Any())
                    return null;

                var cartList = cartItems.Select(c => new CartItemModel
                {
                    BookId = c.BookId,
                    BookName = c.Book.BookName,
                    Quantity = c.Quantity,
                    Price = c.Price
                }).ToList();

                return new CartSummeryModel
                {
                    Items = cartList,
                    TotalQuantity = cartList.Sum(x => x.Quantity),
                    TotalCost = cartList.Sum(x => x.Price)
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        */



        public CartResponseModel GetCartDetails(string token)
        {
            try
            {
                int userId = _jwtHelper.ExtractUserIdFromJwt(token);
                string role = _jwtHelper.ExtractRoleFromJwt(token);

                if (role.ToLower() != "user")
                    return new CartResponseModel { IsSuccess = false, Message = "Only users are allowed to access the cart." };

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
                    Quantity = c.Quantity,
                    Price = c.Price
                }).ToList();

                return new CartResponseModel
                {
                    IsSuccess = true,
                    Message = "Cart fetched successfully.",
                    Data = new CartSummeryModel
                    {
                        Items = cartList,
                        TotalQuantity = cartList.Sum(x => x.Quantity),
                        TotalCost = cartList.Sum(x => x.Price)
                    }
                };
            }
            catch (Exception ex)
            {
                return new CartResponseModel { IsSuccess = false, Message = $"Internal error: {ex.Message}" };
            }
        }


    }
}
