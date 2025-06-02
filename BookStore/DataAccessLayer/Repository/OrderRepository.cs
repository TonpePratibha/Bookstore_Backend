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
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
   public class OrderRepository:IOrderRepository
    {

        public readonly JwtHelper _jwtHelper;
        public ApplicationDbContext _context;
        private readonly ILogger<OrderRepository> _logger;
        public OrderRepository(JwtHelper jwtHelper, ApplicationDbContext context, ILogger<OrderRepository> logger)
        {


            _jwtHelper = jwtHelper;
            _context = context;
            _logger = logger;

        }

      



        public OrderResponse PlaceOrder(string token)
        {
            _logger.LogInformation("PlaceOrder method called.");
            var userId = _jwtHelper.ExtractUserIdFromJwt(token);
            var userRole = _jwtHelper.ExtractRoleFromJwt(token);

            if (userRole != "user")
            {
                _logger.LogWarning("Unauthorized role '{Role}' attempted to place an order.", userRole);
                throw new UnauthorizedAccessException("Only users are allowed to place orders.");
            }

            var cartItems = _context.Cart
                                    .Where(c => c.PurchasedBy == userId && !c.IsPurchased)
                                    .Include(c => c.Book)
                                    .Include(c => c.User)
                                    .ToList();

            if (cartItems.Count == 0)
            {
                _logger.LogWarning("User {UserId} attempted to place an order with an empty cart.", userId);
                throw new InvalidOperationException("No items in cart to place order.");
            }

            var orderResponses = new List<OrderResponseModel>();

            foreach (var cartItem in cartItems)
            {
                var book = cartItem.Book;

               
                if (book.Quantity < cartItem.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for BookId {BookId}. Available: {Available}, Requested: {Requested}",
                                           book.Id, book.Quantity, cartItem.Quantity);
                    throw new InvalidOperationException($"Not enough quantity for book '{book.BookName}'. Available: {book.Quantity}, Requested: {cartItem.Quantity}");
                }

              
                book.Quantity -= cartItem.Quantity;

                var order = new OrderDetails
                {
                    OrderedBy = userId,
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    TotalPrice = cartItem.Price,
                    Orderdate = DateTime.UtcNow
                };

                _context.orderDetails.Add(order);
                _context.SaveChanges();

                _logger.LogInformation("Order placed for BookId {BookId} by UserId {UserId}", book.Id, userId);
                var orderResponse = new OrderResponseModel
                {
                    OrderId = order.Id,
                    OrderedBy = order.OrderedBy,
                    UserFirstName = cartItem.User.FirstName,
                    UserLastName = cartItem.User.LastName,
                    UserEmail = cartItem.User.Email,
                    BookId = cartItem.BookId,
                    BookName = book.BookName,
                    BookImage = book.BookImage,
                    Author = book.Author,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity,
                    OrderDate = order.Orderdate
                };

                orderResponses.Add(orderResponse);
            }

            _context.Cart.RemoveRange(cartItems);
            _context.SaveChanges();

            _logger.LogInformation("Order process completed successfully for UserId {UserId}", userId);
            return new OrderResponse
            {
                Message = "Order placed successfully.",
                Orders = orderResponses
            };
        }



        public List<OrderItemresponse> GetOrdersByUser(string token)
        {
            _logger.LogInformation("GetOrdersByUser called.");

            var userId = _jwtHelper.ExtractUserIdFromJwt(token);
            string role = _jwtHelper.ExtractRoleFromJwt(token);

            if (role.ToLower() != "user")

                throw new UnauthorizedAccessException("Only users can view orders");

            var orders = _context.orderDetails
                .Include(o => o.Book)
                .Where(o => o.OrderedBy == userId)
                .OrderByDescending(o => o.Orderdate)
                .ToList();

            if (orders == null || orders.Count == 0)
                return new List<OrderItemresponse>();

            _logger.LogInformation("Fetched {Count} orders for UserId {UserId}", orders.Count, userId);


            var response = orders.Select(o => new OrderItemresponse
            {
                BookName = o.Book.BookName,
                BookImage=o.Book.BookImage,
                Author = o.Book.Author,
                Quantity = o.Quantity,
                PricePerItem = o.TotalPrice / o.Quantity,
                TotalPrice = o.TotalPrice,
                Orderdate=o.Orderdate,
            }).ToList();

            return response;
        }




    }
}
