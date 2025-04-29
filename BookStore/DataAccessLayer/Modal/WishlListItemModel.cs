using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
   public class WishlListItemModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public decimal? DiscountPrice { get; set; }
        public int? Quantity { get; set; }

        public string BookImage { get; set; }
    }
}
