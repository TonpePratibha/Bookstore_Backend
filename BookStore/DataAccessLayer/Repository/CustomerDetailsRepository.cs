
using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
   public class CustomerDetailsRepository:ICustomerDetailsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<CustomerDetailsRepository> _logger;

        public CustomerDetailsRepository(ApplicationDbContext context, JwtHelper jwtHelper, ILogger<CustomerDetailsRepository> logger)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }



        public CustomerDetails AddCustomerDetails(CustomerDetailsModel model, string token)
        {
            _logger.LogInformation("AddCustomerDetails called by token.");
            int userId = _jwtHelper.ExtractUserIdFromJwt(token);
            string role = _jwtHelper.ExtractRoleFromJwt(token);

            if (role != "user")
            {
                _logger.LogWarning("Unauthorized access attempt to AddCustomerDetails by role: {Role}", role);
                throw new UnauthorizedAccessException("Only Users can add customer details.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogWarning("User not found with Id: {UserId}", userId);
                throw new Exception("User not found.");
            }

            var customer = new CustomerDetails
            {
                FullName = model.FullName,
                Mobile = model.Mobile,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Type = model.Type,
                UserId = userId
            };

            _context.customerDetails.Add(customer);
            _context.SaveChanges();
            _logger.LogInformation("Customer details added successfully for UserId: {UserId}", userId);
            return customer;
        }


        public List<CustomerDetailsResponse> GetAllCustomerDetails(string token)
        {
            _logger.LogInformation("GetAllCustomerDetails called by token.");
            string role = _jwtHelper.ExtractRoleFromJwt(token);

            if (!string.Equals(role, "User", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Unauthorized access attempt to GetAllCustomerDetails by role: {Role}", role);
                throw new UnauthorizedAccessException("Only Users can access customer details.");
            }

            var customers = _context.customerDetails
                .Select(c => new CustomerDetailsResponse
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    FullName = c.FullName,
                    Mobile = c.Mobile,
                    Address = c.Address,
                    City = c.City,
                    State = c.State,
                    Type = c.Type
                })
                .ToList();
            _logger.LogInformation("Fetched {Count} customer details.", customers.Count);
            return customers;
        }


    }
}
