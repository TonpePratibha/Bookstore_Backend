using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
 public class Book
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(255)]
        public string? BookName { get; set; }
        [StringLength(255)]
        public string? Author { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Price { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? DiscountPrice { get; set; }
        public int? Quantity { get; set; }
        [StringLength(500)]
        public string? BookImage { get; set; }
        [StringLength(50)]
        public string? AdminUserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
    }
}
