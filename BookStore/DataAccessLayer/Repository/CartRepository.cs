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
    }
}
