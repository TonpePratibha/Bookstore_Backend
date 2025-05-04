
using DataAccessLayer.Entity;
using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Interface
{
    public interface ICustomerDetailsService
    {
        CustomerDetails AddCustomerDetails(CustomerDetailsModel model, string token);
        public List<CustomerDetailsResponse> GetAllCustomerDetails(string token);
    }
}
