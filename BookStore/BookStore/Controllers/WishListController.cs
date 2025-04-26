using BuisnessLayer.Interface;
using BuisnessLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishListController : ControllerBase
    {

        public readonly IWishListService _wishListService;

        public WishListController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }

        [HttpPost]
        public IActionResult AddToWishList(int bookId) {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Authorization token is missing. Please provide a valid token." });
                }

                var wishlistItem = _wishListService.AddToWishList(token, bookId);

                if (wishlistItem == null)
                {
                    return BadRequest(new { message = "Could not add to  wishlist.book already presentin wishlist or Possible reasons: invalid token, book/user not found, or only users have access admin can't add cart" });
                }

                return Ok(new
                {
                    message = "Book added to wishlist successfully.",
                    data = wishlistItem
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An unexpected error occurred: {ex.Message}" });
            }
        }



        [HttpDelete]

        public IActionResult RemoveFromWishList(int bookId)
        {

            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Authorization token is missing.");

                var result = _wishListService.RemoveFromWishlist(token, bookId);

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



        [HttpGet]
        public IActionResult getWishListDetals() {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Authorization token is missing.");

                var response = _wishListService.GetWishListDetails(token);

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
