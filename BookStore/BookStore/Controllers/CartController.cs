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



    }
}
