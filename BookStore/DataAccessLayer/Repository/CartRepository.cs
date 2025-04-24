using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
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

        public string AddToCart(string token, CartModel cartModel)
        {
            try
            {
                var role = _jwtHelper.ExtractRoleFromJwt(token);
                var userId = _jwtHelper.ExtractUserIdFromJwt(token);

                if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(token))
                    return "Token missing or invalid.";

                if (role.ToLower() != "user")
                    return "Only users can add to cart.";

                // Map DTO to entity
                var cart = new Cart
                {
                    PurchasedBy = userId,
                    BookId = cartModel.BookId,
                    Quantity = cartModel.Quantity,
                    Price = cartModel.Price,
                    IsPurchased = false
                };

                _context.Cart.Add(cart);
                _context.SaveChanges();

                return "Book added to cart successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");

                return $"Error: {ex.Message}";
                
            }
        }

    }
}
