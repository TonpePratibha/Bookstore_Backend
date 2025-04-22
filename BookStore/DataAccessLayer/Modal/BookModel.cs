using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
   public class BookModel
    {
        
        
        public string? BookName { get; set; }
      
        public string? Author { get; set; }
        public string? Description { get; set; }
      
        public decimal? Price { get; set; }
       
        public decimal? DiscountPrice { get; set; }
        public int? Quantity { get; set; }
       
        public string? BookImage { get; set; }
     
        public string? AdminUserId { get; set; }
        
        public DateTime? CreatedAt { get; set; }
     
        public DateTime? UpdatedAt { get; set; }
    }
}
