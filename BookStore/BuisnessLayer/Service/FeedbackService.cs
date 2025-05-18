using BuisnessLayer.Interface;
using DataAccessLayer.Interface;
using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Service
{
    public class FeedbackService:IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        public FeedbackService( IFeedbackRepository feedbackRepository) { 
            _feedbackRepository = feedbackRepository;
        }
        public FeedbackResponseModel AddFeedback(FeedbackRequestModel model, string jwtToken) { 
            return _feedbackRepository.AddFeedback(model, jwtToken);

        }

        public List<FeedbackResponseModel> GetRecentFeedbacksByBookId(int bookId) {
            return _feedbackRepository.GetRecentFeedbacksByBookId(bookId);

        
        }
    }
}
