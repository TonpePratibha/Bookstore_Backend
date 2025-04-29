using BuisnessLayer.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;

        public WishListController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }

        [HttpPost]
        public IActionResult AddToWishList(int bookId)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { success = false, message = "Authorization token is missing." });
                }

                var wishlistItem = _wishListService.AddToWishList(token, bookId);

                if (wishlistItem == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Could not add to wishlist. Book may already exist or token is invalid."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Book added to wishlist successfully.",
                    data = wishlistItem
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [HttpDelete]
        public IActionResult RemoveFromWishList(int bookId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { success = false, message = "Authorization token is missing." });

                var result = _wishListService.RemoveFromWishlist(token, bookId);

                if (result.Contains("Unauthorized"))
                    return Unauthorized(new { success = false, message = result });

                if (result.Contains("not found"))
                    return NotFound(new { success = false, message = result });

                if (result.Contains("not zero"))
                    return BadRequest(new { success = false, message = result });

                return Ok(new { success = true, message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult GetWishListDetails()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { success = false, message = "Authorization token is missing." });

                var response = _wishListService.GetWishListDetails(token);

                if (!response.IsSuccess)
                {
                    if (response.Message.Contains("users"))
                        return Unauthorized(new { success = false, message = response.Message });

                    if (response.Message.Contains("empty") || response.Message.Contains("not found"))
                        return NotFound(new { success = false, message = response.Message });

                    return BadRequest(new { success = false, message = response.Message });
                }

                return Ok(new
                {
                    success = true,
                    message = response.Message,
                    data = response.Data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
