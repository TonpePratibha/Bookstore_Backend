using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public class OrderDetails
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int OrderedBy { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }



        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime Orderdate { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }
        public virtual Book Book { get; set; }
    }
}
