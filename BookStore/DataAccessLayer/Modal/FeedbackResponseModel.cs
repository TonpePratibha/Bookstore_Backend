using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
    public class FeedbackResponseModel
    {
         public int Id { get; set; }
    public int BookId { get; set; }
        public string Bookname { get; set; }
        public int AddedBy { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Rating { get; set; }
    public string Review { get; set; }
   

    }
}
