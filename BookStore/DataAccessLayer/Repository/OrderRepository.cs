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
   public class OrderRepository:IOrderRepository
    {

        public readonly JwtHelper _jwtHelper;
        public ApplicationDbContext _context;
        public OrderRepository(JwtHelper jwtHelper, ApplicationDbContext context)
        {


            _jwtHelper = jwtHelper;
            _context = context;


        }

        /*
                public OrderResponse PlaceOrder(string token)
                {

                    var userId = _jwtHelper.ExtractUserIdFromJwt(token);
                    var userRole = _jwtHelper.ExtractRoleFromJwt(token);


                    if (userRole != "user")
                    {
                        throw new UnauthorizedAccessException("Only users are allowed to place orders.");
                    }


                    var cartItems = _context.Cart
                                            .Where(c => c.PurchasedBy == userId && !c.IsPurchased)
                                            .Include(c => c.Book) // Include Book details
                                            .Include(c => c.User) // Include User details
                                            .ToList();

                    if (cartItems.Count == 0)
                    {
                        throw new InvalidOperationException("No items in cart to place order.");
                    }

                    var orderResponses = new List<OrderResponseModel>();


                    foreach (var cartItem in cartItems)
                    {

                        var order = new OrderDetails
                        {
                            OrderedBy = userId,
                            BookId = cartItem.BookId,
                            Quantity = cartItem.Quantity,
                            TotalPrice = cartItem.Price,
                            Orderdate = DateTime.UtcNow
                        };


                        _context.orderDetails.Add(order);


                        var orderResponse = new OrderResponseModel
                        {
                            OrderId = order.Id,
                            OrderedBy = order.OrderedBy,
                            UserFirstName = cartItem.User.FirstName,
                            UserLastName = cartItem.User.LastName,
                            UserEmail = cartItem.User.Email,
                            BookId = cartItem.BookId,
                            BookName = cartItem.Book.BookName,
                            BookImage=cartItem.Book.BookImage,
                            Author = cartItem.Book.Author,
                            Price = cartItem.Price,
                            Quantity = cartItem.Quantity,
                            OrderDate = order.Orderdate
                        };


                        orderResponses.Add(orderResponse);
                    }


                    _context.Cart.RemoveRange(cartItems);


                    _context.SaveChanges();


                    return new OrderResponse
                    {
                        Message = "Order placed successfully.",
                        Orders = orderResponses
                    };
                }
        */





        public OrderResponse PlaceOrder(string token)
        {
            var userId = _jwtHelper.ExtractUserIdFromJwt(token);
            var userRole = _jwtHelper.ExtractRoleFromJwt(token);

            if (userRole != "user")
            {
                throw new UnauthorizedAccessException("Only users are allowed to place orders.");
            }

            var cartItems = _context.Cart
                                    .Where(c => c.PurchasedBy == userId && !c.IsPurchased)
                                    .Include(c => c.Book)
                                    .Include(c => c.User)
                                    .ToList();

            if (cartItems.Count == 0)
            {
                throw new InvalidOperationException("No items in cart to place order.");
            }

            var orderResponses = new List<OrderResponseModel>();

            foreach (var cartItem in cartItems)
            {
                var book = cartItem.Book;

               
                if (book.Quantity < cartItem.Quantity)
                {
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

            return new OrderResponse
            {
                Message = "Order placed successfully.",
                Orders = orderResponses
            };
        }



        public List<OrderItemresponse> GetOrdersByUser(string token)
        {
           
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
