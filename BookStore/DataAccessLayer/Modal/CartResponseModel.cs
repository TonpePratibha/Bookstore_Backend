using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
   public class CartResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public CartSummeryModel Data { get; set; }
    }
}
