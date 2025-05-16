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

        [HttpPost]
        public IActionResult AddToCart(int bookId)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Authorization token is missing. Please provide a valid token." });
                }

                var cartItem = _cartService.AddToCart(token, bookId);

                if (cartItem == null)
                {
                    return BadRequest(new { message = "Could not add to cart. Possible reasons: invalid token, book/user not found, or only users have access admin can't add cart" });
                }

                return Ok(new
                {
                    message = "Book added to cart successfully.",
                    data = cartItem
                });
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
                {
                    return Unauthorized(new { message = "Authorization token is missing." });
                }

                var result = _cartService.UpdateCartQuantity(token, bookId, quantity);

                if (result == null)
                {
                    return BadRequest(new { message = "Update failed. Either you're unauthorized, or the item/book was not found or quantity is invalid / admin cant have access to cart" });
                }

                return Ok(new
                {
                    message = "Cart updated successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
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

                return Ok(new { result });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

     
      


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
