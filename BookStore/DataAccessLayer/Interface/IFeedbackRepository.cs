using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IFeedbackRepository
    {
        public FeedbackResponseModel AddFeedback(FeedbackRequestModel model, string jwtToken);
        public List<FeedbackResponseModel> GetRecentFeedbacksByBookId(int bookId);
    }
}
