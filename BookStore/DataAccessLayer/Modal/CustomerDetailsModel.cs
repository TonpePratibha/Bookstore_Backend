
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace DataAccessLayer.Modal
{


   
        public class CustomerDetailsModel
        {

        
        [Required(ErrorMessage = "Full Name is required")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Mobile number is required")]
            [Phone(ErrorMessage = "Invalid phone number format")]
            [StringLength(15, ErrorMessage = "Mobile number cannot exceed 15 digits")]
            public string Mobile { get; set; }

            [Required(ErrorMessage = "Address is required")]
            [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
            public string Address { get; set; }

            [Required(ErrorMessage = "City is required")]
            [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
            public string City { get; set; }

            [Required(ErrorMessage = "State is required")]
            [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
            public string State { get; set; }

            [Required(ErrorMessage = "Type is required")]
            [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
            public string Type { get; set; }
        }
    }


