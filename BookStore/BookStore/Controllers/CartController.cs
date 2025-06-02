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
        private readonly ILogger<CartController> _logger;
        public CartController(ICartService cartService, ILogger<CartController> logger )
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult AddToCart(int bookId)
        {
            _logger.LogInformation("AddToCart initiated for Book ID: {BookId}", bookId);
            try
            {

                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Missing authorization token for AddToCart.");
                    return Unauthorized(new { message = "Authorization token is missing. Please provide a valid token." });
                }

                var cartItem = _cartService.AddToCart(token, bookId);

                if (cartItem == null)
                {
                    _logger.LogWarning("AddToCart failed - invalid token or book/user not found.");
                    return BadRequest(new { message = "Could not add to cart. Possible reasons: invalid token, book/user not found, or only users have access admin can't add cart" });
                }
                _logger.LogInformation("Book successfully added to cart.");
                return Ok(new
                {
                    message = "Book added to cart successfully.",
                    data = cartItem
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in AddToCart.");
                return StatusCode(500, new { message = $"An unexpected error occurred: {ex.Message}" });
            }
        }


       
        [HttpPut("{bookId}")]
        public IActionResult UpdateCart(int bookId, [FromQuery] int quantity)
        {
            _logger.LogInformation("UpdateCart called for Book ID: {BookId} with Quantity: {Quantity}", bookId, quantity);
            try
            {
                var token = Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Missing authorization token for UpdateCart.");
                    return Unauthorized(new { message = "Authorization token is missing." });
                }

                var result = _cartService.UpdateCartQuantity(token, bookId, quantity);

                if (result == null)
                {
                    _logger.LogWarning("UpdateCart failed due to invalid token or data.");
                    return BadRequest(new { message = "Update failed. Either you're unauthorized, or the item/book was not found or quantity is invalid / admin cant have access to cart" });
                }
                _logger.LogInformation("Cart updated successfully for Book ID: {BookId}", bookId);
               
                return Ok(new
                {
                    message = "Cart updated successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in UpdateCart.");
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }



        [HttpDelete("{bookId}")]
        public IActionResult DeleteCartItemIfQuantityZero(int bookId)
        {
            _logger.LogInformation("DeleteCartItemIfQuantityZero called for Book ID: {BookId}", bookId);

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
                _logger.LogInformation("Cart item deleted successfully.");
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in DeleteCartItemIfQuantityZero.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

     
      


        [HttpGet]
        public IActionResult GetCart()
        {
            _logger.LogInformation("GetCart called");
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Authorization token is missing.");

                var response = _cartService.GetCartDetails(token);

                if (!response.IsSuccess)
                {
                    _logger.LogWarning("GetCart failed: {Message}", response.Message);
                    if (response.Message.Contains("users"))
                        return Unauthorized(response.Message);
                    else if (response.Message.Contains("empty") || response.Message.Contains("not found"))
                        return NotFound(response.Message);
                    else
                        return BadRequest(response.Message); // For other issues
                }
                _logger.LogInformation("Cart details retrieved successfully.");
                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetCart.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
