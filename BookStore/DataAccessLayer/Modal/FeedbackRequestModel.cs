using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
   public class FeedbackRequestModel
    {
        
            [Required]
            public int BookId { get; set; }

            [Required]
            [Range(1, 5)]
            public int Rating { get; set; }

            [Required]
            [StringLength(1000)]
            public string Review { get; set; }
        }

    
}
