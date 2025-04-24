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

        /*
                [HttpPost]
                [Authorize]
                public IActionResult AddToCart([FromBody] CartModel cartModel)
                {
                    try
                    {
                        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

                        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                        {
                            return Unauthorized("Authorization token is missing or invalid.");
                        }

                        var token = authHeader.Replace("Bearer ", "");

                        var result = _cartService.AddToCart(token, cartModel);

                        if (result.Contains("Only users"))
                        {
                            return Forbid("Access denied: Only users are allowed to add items to cart.");
                        }

                        if (result.StartsWith("Error"))
                        {
                            return StatusCode(500, new { error = "Internal server error", detail = result });
                        }

                        if (result.StartsWith("Business error"))
                        {
                            return StatusCode(500, new { error = "Application logic failed", detail = result });
                        }

                        return Ok(new { message = "Book added to cart successfully." });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { error = "Unhandled exception", detail = ex.Message });
                    }
                }
                */

        [HttpPost]
       
        public IActionResult AddToCart([FromBody] CartModel cartModel)
        {
            try
            {
              
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

              
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Authorization token is missing. Please provide a valid token." });
                }

              
              
                var result = _cartService.AddToCart(token, cartModel);

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



    }
}
