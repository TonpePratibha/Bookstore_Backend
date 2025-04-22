using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
    public class AdminModel
    {
        //public int Id { get; set; }


      //  [Required(ErrorMessage = "FirstName required")]
        //[RegularExpression("^[a-zA-Z]$", ErrorMessage = "only letters are accepted")]
        public string FirstName { get; set; }
      //  [Required(ErrorMessage = "LastName required")]
        //[RegularExpression("^[a-zA-Z]$", ErrorMessage = "  only letters are accepted")]
        public string LastName { get; set; }
      //  [Required(ErrorMessage = "Email required")]
        //[EmailAddress(ErrorMessage = "Invalid Email format")]

        public string Email { get; set; }
     //   [Required(ErrorMessage = "Password required")]
       // [MinLength(6, ErrorMessage = "min 6 characters required")]

        public string Password { get; set; }

    }
}
