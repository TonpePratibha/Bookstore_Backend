using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Interface
{
    public interface IFeedbackService
    {
        public FeedbackResponseModel AddFeedback(FeedbackRequestModel model, string jwtToken);
        public List<FeedbackResponseModel> GetRecentFeedbacksByBookId(int bookId);
    }
}
