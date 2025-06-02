
using BuisnessLayer.Interface;
using DataAccessLayer.Modal;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerDetailsController : ControllerBase
    {


        private readonly ICustomerDetailsService _customerDetailsService;
        private readonly ILogger<CustomerDetailsController> _logger;


        public CustomerDetailsController(ICustomerDetailsService customerDetailsService, ILogger<CustomerDetailsController> logger)
        {
            _customerDetailsService=customerDetailsService;
            _logger = logger;

        }


        [HttpPost]
        public IActionResult AddCustomerDetails([FromBody] CustomerDetailsModel model)
        {
            _logger.LogInformation("AddCustomerDetails API called.");
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                _logger.LogDebug("Token extracted successfully.");

                var result = _customerDetailsService.AddCustomerDetails(model, token);
                _logger.LogInformation("Customer details added successfully.");
                return Ok(new { message = "Customer details added successfully", data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access in AddCustomerDetails.");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding customer details.");
                return BadRequest(new { message = "Customer details not added.", error = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult GetAllCustomerDetails()
        {
            _logger.LogInformation("GetAllCustomerDetails API called.");
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                _logger.LogDebug("Token extracted successfully.");
                var result = _customerDetailsService.GetAllCustomerDetails(token);
                return Ok(new { message = "Customer details fetched successfully", data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access in GetAllCustomerDetails.");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching customer details.");
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }


    }
}
