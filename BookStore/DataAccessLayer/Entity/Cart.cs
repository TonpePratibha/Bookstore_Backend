using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
  public class Cart
    {

    


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int PurchasedBy { get; set; }  // FK to User

        [ForeignKey("Book")]
        public int BookId { get; set; }  // FK to Book

        public int Quantity { get; set; }

        public Decimal Price { get; set; }

        public bool IsPurchased { get; set; }=false;

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Book Book { get; set; }
    }

}

