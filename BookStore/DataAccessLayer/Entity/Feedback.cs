using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int AddedBy { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        [Required]
        public int rating { get; set; }


        [Required]
        [StringLength(1000, ErrorMessage = "Review cannot exceed 1000 characters.")]
        public string review { get; set; }


        public virtual User User { get; set; }
        public virtual Book Book { get; set; }

    }
}
