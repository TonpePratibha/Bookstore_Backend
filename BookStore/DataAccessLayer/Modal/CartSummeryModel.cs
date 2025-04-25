using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
   public class CartSummeryModel
    {
        public UserDetailsModel User { get; set; }
        public List<CartItemModel> Items { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalCost { get; set; }

        
    }
}
