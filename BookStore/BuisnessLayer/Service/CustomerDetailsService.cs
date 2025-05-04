using BuisnessLayer.Interface;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;

using DataAccessLayer.Modal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Service
{
    public class CustomerDetailsService : ICustomerDetailsService
    {
        private readonly ICustomerDetailsRepository _repository;

        public CustomerDetailsService(ICustomerDetailsRepository repository)
        {
            _repository = repository;
        }

        public CustomerDetails AddCustomerDetails(CustomerDetailsModel model, string token)

        {
            return _repository.AddCustomerDetails(model, token);// Logic handled in repo
        }

        public List<CustomerDetailsResponse> GetAllCustomerDetails(string token)
        {
            return _repository.GetAllCustomerDetails(token);
        }

    }
}
