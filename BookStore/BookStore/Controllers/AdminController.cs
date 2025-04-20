using BuisnessLayer.Interface;
using BuisnessLayer.Service;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace BookStore.Controllers
{
    [Route("api/admin")]
    [ApiController]
 
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;


        public AdminController(IAdminService adminService)
        {

            _adminService = adminService;
        }
         
          [HttpPost]
          public IActionResult Register([FromBody] AdminModel adminModel)
          {
              try
              {
                  if (!ModelState.IsValid)
                  {
                      var errors = ModelState
                          .Where(e => e.Value.Errors.Count > 0)
                          .ToDictionary(
                              kvp => kvp.Key,
                              kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                          );

                      return BadRequest(new { message = "Validation failed", errors });
                  }

                  var admin = _adminService.RegisterAdmin(adminModel);
                  return Ok(new { message = "Admin registered successfully.", newAdmin = admin });
              }
              catch (Exception ex)
              {
                  return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
              }
          }

          

       


        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminLogin adminLoginModel) {


            try {

                var response = _adminService.ValidateAdmin(adminLoginModel);



                if (response == null)
                {
                    return Unauthorized(new { Error = "unauthorized invalid email or password" });
                }
                return Ok(new { Response=response
                });


            }
            catch (Exception ex) {
                return BadRequest(new { Error = ex.Message });

            }





        }
        [HttpGet("{id}")]
        [Authorize]
            public IActionResult GetAdminById(int id) {
            try
            {
                var admin = _adminService.GetAdminById(id);
                if (admin == null)
                {
                    return NotFound(new { error = "admin not found" });
                }
                return Ok(admin);
            }
            catch (Exception  ex) {
                return BadRequest(new { error = "unauthorized user" });
            }
                 
        }

        [HttpDelete("{id}")]
        public IActionResult deleteAdmin(int id)
        {
            try {
                 _adminService.DeleteAdmin(id);
                return Ok("admin deleted");



            }
            catch (Exception ex) {
                return BadRequest(new {ex.Message });
            }
        }



        [HttpPut("{id}")]
        public IActionResult UpdateAdmin(int id, [FromBody] AdminModel model)
        {
            try
            {
                _adminService.UpdateAdmin(id, model);
                return Ok(new { Message = "Admin updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPaswordModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {

                return BadRequest("Email is required.");
            }
            try
            {

                _adminService.SendResetPasswordEmail(model.Email);


                return Ok("Password reset email sent.");
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { Message = "Authorization header is missing or invalid" });
            }

            var token = authHeader.Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { Message = "Token is required" });
            }

            try
            {
                var result = _adminService.ResetPassword(token, model.NewPassword);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {

                return BadRequest(new { Message = ex.Message });
            }
        }

       

        [HttpPost("accesslogin")]
        public IActionResult AccessLogin(AdminLogin login)
        {
            try
            {
                var result = _adminService.AccesstokenLogin(login);
                if (result == null) return Unauthorized("Invalid credentials.");
                return Ok(result); 
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(RefreshRequest request)
        {
            try
            {
                var result = _adminService.RefreshAccessToken(request.RefreshToken);
                if (result == null) return Unauthorized("Invalid or expired refresh token.");
                return Ok(result); 
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
