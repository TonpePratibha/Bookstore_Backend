using BuisnessLayer.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;
        private readonly ILogger<WishListController> _logger;
        public WishListController(IWishListService wishListService, ILogger<WishListController> logger)
        {
            _wishListService = wishListService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult AddToWishList(int bookId)
        {
            _logger.LogInformation("AddToWishList called for BookId: {BookId}", bookId);
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Authorization token is missing.");
                    return Unauthorized(new { success = false, message = "Authorization token is missing." });
                }

                var wishlistItem = _wishListService.AddToWishList(token, bookId);

                if (wishlistItem == null)
                {
                    _logger.LogWarning("Failed to add book {BookId} to wishlist. Book may already exist or token is invalid.", bookId);
                    return BadRequest(new
                    {
                        success = false,
                        message = "Could not add to wishlist. Book may already exist or token is invalid."
                    });
                }
                _logger.LogInformation("Book {BookId} added to wishlist successfully.", bookId);
                return Ok(new
                {
                    success = true,
                    message = "Book added to wishlist successfully.",
                    data = wishlistItem
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding book {BookId} to wishlist.", bookId);
                return StatusCode(500, new { success = false, message = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [HttpDelete]
        public IActionResult RemoveFromWishList(int bookId)
        {
            _logger.LogInformation("RemoveFromWishList called for BookId: {BookId}", bookId);
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
                _logger.LogError(ex, "Internal server error while removing book {BookId} from wishlist.", bookId);
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult GetWishListDetails()
        {
            _logger.LogInformation("GetWishListDetails called.");
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { success = false, message = "Authorization token is missing." });

                var response = _wishListService.GetWishListDetails(token);

                if (!response.IsSuccess)
                {
                    _logger.LogWarning("Failed to retrieve wishlist: {Message}", response.Message);
                    if (response.Message.Contains("users"))
                        return Unauthorized(new { success = false, message = response.Message });

                    if (response.Message.Contains("empty") || response.Message.Contains("not found"))
                        return NotFound(new { success = false, message = response.Message });

                    return BadRequest(new { success = false, message = response.Message });
                }

                _logger.LogInformation("Wishlist retrieved successfully.");
                return Ok(new
                {
                    success = true,
                    message = response.Message,
                    data = response.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error while retrieving wishlist.");
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
