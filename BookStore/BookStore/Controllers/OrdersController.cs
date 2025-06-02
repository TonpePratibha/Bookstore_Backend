using BuisnessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        public readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }
        [HttpPost("placeorder")]
        
        public IActionResult PlaceOrder()
        {
            _logger.LogInformation("PlaceOrder API called.");
            try
            {
                
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Authorization token missing in PlaceOrder.");
                    return Unauthorized(new { message = "Authorization token is missing. Please provide a valid token." });
                }

                
                var orderResponse = _orderService.PlaceOrder(token);

                if (orderResponse == null)
                {
                    _logger.LogWarning("PlaceOrder failed: No items in cart or user not found.");
                    return BadRequest(new { message = "Could not place the order. Possible reasons: no items in cart, or user not found." });
                }

                _logger.LogInformation("Order placed successfully.");
                return Ok(new
                {
                    message = orderResponse.Message,
                    data = orderResponse.Orders
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access in PlaceOrder.");
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation during PlaceOrder.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in PlaceOrder.");
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

       

        [HttpGet]
        public IActionResult GetUserOrders()
        {
            _logger.LogInformation("GetUserOrders API called.");
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Authorization token missing in GetUserOrders.");
                    return BadRequest(new
                    {
                        Status = 400,
                        Success = false,
                        Message = "Authorization token is missing",
                        Data = (object)null
                    });
                }

               
                var orders = _orderService.GetOrdersByUser(token);

                
                if (orders == null || orders.Count == 0)
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Success = false,
                        Message = "No orders found",
                        Data = (object)null
                    });
                }
                _logger.LogInformation("Orders fetched successfully.");
                return Ok(new
                {
                    Status = 200,
                    Success = true,
                    Message = "Orders fetched successfully",
                    Data = orders
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access in GetUserOrders.");
                // Handle case where the user is not authorized (i.e., not a user role)
                return StatusCode(403,new
                {
                    Status = 403,
                    Success = false,
                    Message = "Only users can access their orders. Admins are not allowed.",
                    Data = (object)null
                });
            }
            catch (Exception)
            {
                // Handle general errors
                _logger.LogError( "Error fetching user orders.");
                return StatusCode(500, new
                {
                    Status = 500,
                    Success = false,
                    Message = "Something went wrong while fetching orders",
                    Data = (object)null
                });
            }
        }

    }
}
