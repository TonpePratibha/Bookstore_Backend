using BuisnessLayer.Interface;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookStore.Controllers
{
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private IFeedbackService _feedbackService;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(IFeedbackService feedbackService, ILogger<FeedbackController> logger) {
           _feedbackService = feedbackService;
            _logger = logger;
        }


        [HttpPost]
     
        public IActionResult AddFeedback([FromBody] FeedbackRequestModel model)
        {
            _logger.LogInformation("AddFeedback API called.");
            try
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                {
                    _logger.LogWarning("Authorization header missing in AddFeedback.");
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Authentication required."
                    });
                }

                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                _logger.LogDebug("Authorization token extracted.");
                var result = _feedbackService.AddFeedback(model, token);

                if (result == null)
                {
                    // Token was valid, but role was not "User"
                    _logger.LogWarning("AddFeedback failed: Only users are allowed.");
                    return StatusCode(403, new
                    {
                        success = false,
                        message = "Only users can add feedback."
                    });
                }
                _logger.LogInformation("Feedback added successfully.");
                return Ok(new
                {
                    success = true,
                    message = "Feedback added successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding feedback.");
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
            _logger.LogInformation("GetRecentFeedbacksByBookId API called for BookId: {BookId}", bookId);
            try
            {
                var feedbacks = _feedbackService.GetRecentFeedbacksByBookId(bookId);

                if (feedbacks == null || feedbacks.Count == 0)
                {
                    _logger.LogWarning("No feedbacks found for BookId: {BookId}", bookId);
                    return NotFound(new
                    {
                        success = false,
                        message = "No feedbacks found for this book."
                    });
                }
                _logger.LogInformation("Feedbacks retrieved successfully for BookId: {BookId}", bookId);
                return Ok(new
                {
                    success = true,
                    message = "Recent feedbacks retrieved successfully.",
                    data = feedbacks
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving feedbacks for BookId: {BookId}", bookId);
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
