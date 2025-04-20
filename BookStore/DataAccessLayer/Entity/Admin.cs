using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "FirstName required")]
        [RegularExpression("^[a-zA-Z]$",ErrorMessage ="for firstname only letters are accepted")]
        public string FirstName { get; set; }             
        [Required(ErrorMessage = "LastName required")]
        [RegularExpression("^[a-zA-Z]$", ErrorMessage = " for lastname only letters are accepted")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]

        public string Email { get; set; }
        [Required(ErrorMessage = "Password required")]
        [MinLength(6, ErrorMessage = "min 6 characters required")]
        
        public string Password { get; set; }
       
        public string Role { get; set; }
    }
}
