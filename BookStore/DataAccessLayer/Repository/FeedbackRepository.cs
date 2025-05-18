using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
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

            public FeedbackRepository(ApplicationDbContext context, JwtHelper jwtHelper)
            {
                _context = context;
                _jwtHelper = jwtHelper;
            }

        public FeedbackResponseModel AddFeedback(FeedbackRequestModel model, string jwtToken)
        {
            try
            {
                int userId = _jwtHelper.ExtractUserIdFromJwt(jwtToken);
                string role = _jwtHelper.ExtractRoleFromJwt(jwtToken);

                if (role != "user")
                {
                    return null; // Block non-users
                }

                // Check if user already added feedback for the same book
                bool feedbackExists = _context.feedbacks
                    .Any(f => f.BookId == model.BookId && f.AddedBy == userId);

                if (feedbackExists)
                {
                    throw new InvalidOperationException("Feedback already exists for this book by the user.");
                }

                // Get user details
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
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
                throw;
            }
        }

        public List<FeedbackResponseModel> GetRecentFeedbacksByBookId(int bookId)
        {
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

            return feedbacks;
        }


    }
}
