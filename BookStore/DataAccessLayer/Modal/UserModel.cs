using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
   public class UserModel
    {
      //  public int Id { get; set; }
       
        public string FirstName { get; set; }
       
        public string LastName { get; set; }
       
        public string Email { get; set; }
    
       public string Password { get; set; }
        

    }
}
