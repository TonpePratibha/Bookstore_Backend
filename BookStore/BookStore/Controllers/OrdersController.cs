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

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
       [HttpPost("placeorder")]
        
        public IActionResult PlaceOrder()
        {
            try
            {
                // Extract the token from the Authorization header
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Authorization token is missing. Please provide a valid token." });
                }

                // Call the repository method to place the order using the token
                var orderResponse = _orderService.PlaceOrder(token);

                if (orderResponse == null)
                {
                    return BadRequest(new { message = "Could not place the order. Possible reasons: no items in cart, or user not found." });
                }

                // Return success message along with the order details
                return Ok(new
                {
                    message = orderResponse.Message,
                    data = orderResponse.Orders
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Return error if the user is unauthorized
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Return bad request error if no items are found in the cart or another validation issue
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Return a generic error message for any unexpected error
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        /*

                [HttpGet]
                public IActionResult GetUserOrders()
                {
                    try
                    {

                        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");


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
                        return Unauthorized(new
                        {
                            Status = 401,
                            Success = false,
                            Message = ex.Message,
                            Data = (object)null
                        });
                    }
                    catch (Exception)
                    {
                        return StatusCode(500, new
                        {
                            Status = 500,
                            Success = false,
                            Message = "Something went wrong while fetching orders",
                            Data = (object)null
                        });
                    }
                }
                */


        [HttpGet]
        public IActionResult GetUserOrders()
        {
            try
            {
                // Extract token from the request header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Check if the token is missing
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Success = false,
                        Message = "Authorization token is missing",
                        Data = (object)null
                    });
                }

                // Get orders for the logged-in user via the repository
                var orders = _orderService.GetOrdersByUser(token);

                // If no orders are found
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
