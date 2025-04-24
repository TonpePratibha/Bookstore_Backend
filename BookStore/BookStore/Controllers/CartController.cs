using BuisnessLayer.Interface;
using DataAccessLayer.Entity;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly ICartService _cartService;
        public CartController(ICartService cartService) {
        _cartService = cartService;
        }

       

        [HttpPost("bookId")]
       
        public IActionResult AddToCart(int bookId)
        {
            try
            {
              
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

              
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Authorization token is missing. Please provide a valid token." });
                }

              
              
                var result = _cartService.AddToCart(token, bookId);

                if (result.Contains("Only users"))
                {
                    return Unauthorized(new { message = "Only users can add items to the cart." });
                }

                if (result.StartsWith("Error") || result.StartsWith("Repository error"))
                {
                    return StatusCode(500, new { message = "Internal server error. Please try again later." });
                }

                return Ok(new { message = "Book added to cart successfully." });
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { message = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [HttpPut("{bookId}")]
        public IActionResult UpdateCart(int bookId, [FromQuery] int quantity)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Token is missing or invalid.");

                var result = _cartService.UpdateCartQuantity(token, bookId, quantity);

                if (result.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase))
                    return Unauthorized(result);

                if (result.Contains("Access denied", StringComparison.OrdinalIgnoreCase))
                    return Forbid(result);

                if (result.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(result);

                if (result.Contains("Invalid", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{bookId}")]
        public IActionResult DeleteCartItemIfQuantityZero(int bookId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Authorization token is missing.");

                var result = _cartService.DeleteFromCartIfQuantityZero(token, bookId);

                if (result.Contains("Unauthorized"))
                    return Unauthorized(result);
                else if (result.Contains("not found"))
                    return NotFound(result);
                else if (result.Contains("not zero"))
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /*
        [HttpGet]
        public IActionResult GetCart()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Authorization token is missing.");

                var result = _cartService.GetCartDetails(token);

             

                 if (result == null)
                      return NotFound("Cart is empty or not found.");
                

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        */


        [HttpGet]
        public IActionResult GetCart()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Authorization token is missing.");

                var response = _cartService.GetCartDetails(token);

                if (!response.IsSuccess)
                {
                    if (response.Message.Contains("users"))
                        return Unauthorized(response.Message);
                    else if (response.Message.Contains("empty") || response.Message.Contains("not found"))
                        return NotFound(response.Message);
                    else
                        return BadRequest(response.Message); // For other issues
                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
