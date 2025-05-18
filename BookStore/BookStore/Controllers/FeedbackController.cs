using BuisnessLayer.Interface;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService) {
           _feedbackService = feedbackService;
        }


        [HttpPost]
     
        public IActionResult AddFeedback([FromBody] FeedbackRequestModel model)
        {
            try
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Authentication required."
                    });
                }

                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var result = _feedbackService.AddFeedback(model, token);

                if (result == null)
                {
                    // Token was valid, but role was not "User"
                    return StatusCode(403, new
                    {
                        success = false,
                        message = "Only users can add feedback."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Feedback added successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while adding feedback.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{bookId}")]
        public IActionResult GetRecentFeedbacksByBookId(int bookId)
        {
            try
            {
                var feedbacks = _feedbackService.GetRecentFeedbacksByBookId(bookId);

                if (feedbacks == null || feedbacks.Count == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No feedbacks found for this book."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Recent feedbacks retrieved successfully.",
                    data = feedbacks
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving feedbacks.",
                    error = ex.Message
                });
            }
        }



    }
}
