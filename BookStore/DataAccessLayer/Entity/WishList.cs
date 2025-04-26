using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public class WishList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int AddedBy { get; set; }  // FK to User

        [ForeignKey("Book")]
        public int BookId { get; set; }  // FK to Book


        // Navigation properties
        public virtual User User { get; set; }
        public virtual Book Book { get; set; }
    }
}
