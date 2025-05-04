
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


        public CustomerDetailsController(ICustomerDetailsService customerDetailsService)
        {
            _customerDetailsService=customerDetailsService;


        }


        [HttpPost]
        public IActionResult AddCustomerDetails([FromBody] CustomerDetailsModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var result = _customerDetailsService.AddCustomerDetails(model, token);

                return Ok(new { message = "Customer details added successfully", data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Customer details not added.", error = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult GetAllCustomerDetails()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = _customerDetailsService.GetAllCustomerDetails(token);
                return Ok(new { message = "Customer details fetched successfully", data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }


    }
}
