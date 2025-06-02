using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class FeedbackRepository:IFeedbackRepository
    {
       
            private readonly ApplicationDbContext _context;
            private readonly JwtHelper _jwtHelper;
        private readonly ILogger<FeedbackRepository> _logger;

        public FeedbackRepository(ApplicationDbContext context, JwtHelper jwtHelper, ILogger<FeedbackRepository> logger)
            {
                _context = context;
                _jwtHelper = jwtHelper;
            _logger = logger;
        }

        public FeedbackResponseModel AddFeedback(FeedbackRequestModel model, string jwtToken)
        {
            try
            {
                _logger.LogInformation("AddFeedback called for BookId: {BookId}", model.BookId);
                int userId = _jwtHelper.ExtractUserIdFromJwt(jwtToken);
                string role = _jwtHelper.ExtractRoleFromJwt(jwtToken);

                if (role != "user")
                {
                    _logger.LogWarning("Unauthorized AddFeedback attempt by role: {Role}", role);
                    return null; // Block non-users
                }

                // Check if user already added feedback for the same book
                bool feedbackExists = _context.feedbacks
                    .Any(f => f.BookId == model.BookId && f.AddedBy == userId);

                if (feedbackExists)
                {
                    _logger.LogWarning("User {UserId} already added feedback for BookId {BookId}", userId, model.BookId);
                    throw new InvalidOperationException("Feedback already exists for this book by the user.");
                }

                // Get user details
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with Id: {UserId}", userId);
                    throw new Exception("User not found.");
                }

                var feedback = new Feedback
                {
                    BookId = model.BookId,
                    rating = model.Rating,
                    review = model.Review,
                    AddedBy = userId
                };

                _context.feedbacks.Add(feedback);
                _context.SaveChanges();
                _logger.LogInformation("Feedback added successfully by UserId: {UserId} for BookId: {BookId}", userId, model.BookId);
                return new FeedbackResponseModel
                {
                    Id = feedback.Id,
                    BookId = feedback.BookId,
                    AddedBy = feedback.AddedBy,
                    Firstname = user.FirstName,   // Add
                    Lastname = user.LastName,     // Add
                    Rating = feedback.rating,
                    Review = feedback.review
                };
            }
            catch (Exception)
            {
                _logger.LogError("Error occurred in AddFeedback.");
                throw;
            }
        }

        public List<FeedbackResponseModel> GetRecentFeedbacksByBookId(int bookId)
        {
            _logger.LogInformation("GetRecentFeedbacksByBookId called for BookId: {BookId}", bookId);
            var feedbacks = _context.feedbacks
                .Where(f => f.BookId == bookId)
                .OrderByDescending(f => f.Id) // Or use CreatedDate if you have it
                .Take(3)
                .Join(_context.Users,
                      f => f.AddedBy,
                      u => u.Id,
                      (f, u) => new FeedbackResponseModel
                      {
                          Id = f.Id,
                          BookId = f.BookId,
                          AddedBy = f.AddedBy,
                          Firstname = u.FirstName,
                          Lastname = u.LastName,
                          Rating = f.rating,
                          Review = f.review
                      })
                .ToList();
            _logger.LogInformation("Fetched {Count} recent feedbacks for BookId: {BookId}", feedbacks.Count, bookId);

            return feedbacks;
        }


    }
}
