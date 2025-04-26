using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
  public class WishListResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public WishListSummeryModel Data { get; set; }
    }
}
