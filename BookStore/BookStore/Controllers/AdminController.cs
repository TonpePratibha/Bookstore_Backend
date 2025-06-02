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
        private readonly ILogger<AdminController> _logger;


        public AdminController(IAdminService adminService,ILogger<AdminController> logger)
        {

            _adminService = adminService;
            _logger = logger;
        }
         
          [HttpPost]
          public IActionResult Register([FromBody] AdminModel adminModel)

          {
            _logger.LogInformation("register endpoint called");
              try
              {
                  if (!ModelState.IsValid)
                  {
                    _logger.LogWarning("validation failed for admin registrartion");
                      var errors = ModelState
                          .Where(e => e.Value.Errors.Count > 0)
                          .ToDictionary(
                              kvp => kvp.Key,
                              kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                          );

                      return BadRequest(new { message = "Validation failed", errors });
                  }
                  
                  
                  var admin = _adminService.RegisterAdmin(adminModel);
                _logger.LogInformation("admin registered successfully",admin);
                  return Ok(new { message = "Admin registered successfully.", newAdmin = admin });
              }
              catch (Exception ex)
              {
                _logger.LogError(ex, "error occured during admin registration");
                  return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
              }
          }

          

       


        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminLogin adminLoginModel) {

            _logger.LogInformation("Login attempt for admin with email: {Email}", adminLoginModel.Email);
            try {

                var response = _adminService.ValidateAdmin(adminLoginModel);



                if (response == null)
                {
                    _logger.LogWarning("Unauthorized login attempt for email: {Email}", adminLoginModel.Email);
                    return Unauthorized(new { Error = "unauthorized invalid email or password" });
                }
                _logger.LogInformation("Admin login successful for email: {Email}", adminLoginModel.Email);
                return Ok(new { Response=response
                });


            }
            catch (Exception ex) {
                _logger.LogError(ex, "Exception occurred during admin login for email: {Email}", adminLoginModel.Email);
                return BadRequest(new { Error = ex.Message });

            }





        }
        [HttpGet("{id}")]
        [Authorize]
            public IActionResult GetAdminById(int id) {
            _logger.LogInformation("Fetching admin with ID: {Id}", id);
            try
            {
                var admin = _adminService.GetAdminById(id);
                if (admin == null)
                {
                    _logger.LogWarning("Admin with ID {Id} not found", id);
                    return NotFound(new { error = "admin not found" });
                }
                _logger.LogInformation("Admin with ID {Id} retrieved successfully", id);
                return Ok(admin);
            }
            catch (Exception  ex) {
                _logger.LogError(ex, "Error fetching admin with ID {Id}", id);
                return BadRequest(new { error = "unauthorized user" });
            }
                 
        }

        [HttpDelete("{id}")]
        public IActionResult deleteAdmin(int id)
        {
            _logger.LogInformation("Attempting to delete admin with ID: {Id}", id);
            try {
                 _adminService.DeleteAdmin(id);
                _logger.LogInformation("Admin with ID {Id} deleted", id);
                return Ok("admin deleted");



            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error deleting admin with ID {Id}", id);
                return BadRequest(new {ex.Message });
            }
        }



        [HttpPut("{id}")]
        public IActionResult UpdateAdmin(int id, [FromBody] AdminModel model)
        {
            _logger.LogInformation("Updating admin with ID: {Id}", id);
            try
            {
                _adminService.UpdateAdmin(id, model);
                _logger.LogInformation("Admin with ID {Id} updated successfully", id);
                return Ok(new { Message = "Admin updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admin with ID {Id}", id);
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPaswordModel model)
        {
            _logger.LogInformation("Forgot password requested for email: {Email}", model.Email);
            if (string.IsNullOrEmpty(model.Email))
            {
                _logger.LogWarning("Forgot password failed: Email is missing");
                return BadRequest("Email is required.");
            }
            try
            {

                _adminService.SendResetPasswordEmail(model.Email);
                _logger.LogInformation("Password reset email sent to {Email}", model.Email);

                return Ok("Password reset email sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email to {Email}", model.Email);
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            _logger.LogInformation("Reset password request received");
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogWarning("Reset password failed: Missing or invalid Authorization header");
                return BadRequest(new { Message = "Authorization header is missing or invalid" });
            }

            var token = authHeader.Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Reset password failed: Token is missing");
                return BadRequest(new { Message = "Token is required" });
            }

            try
            {
                var result = _adminService.ResetPassword(token, model.NewPassword);
                _logger.LogInformation("Password reset successful");
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                return BadRequest(new { Message = ex.Message });
            }
        }

       

        [HttpPost("accesslogin")]
        public IActionResult AccessLogin(AdminLogin login)
        {
            _logger.LogInformation("Access login attempt for email: {Email}", login.Email);
            try
            {
                var result = _adminService.AccesstokenLogin(login);
                if (result == null)
                        {
                    _logger.LogWarning("Access login failed for email: {Email}", login.Email);
                    return Unauthorized("Invalid credentials."); 
                }
                _logger.LogInformation("Access login successful for email: {Email}", login.Email);
                return Ok(result); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during access login for email: {Email}", login.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(RefreshRequest request)
        {
            _logger.LogInformation("Refresh token attempt");
            try
            {
                var result = _adminService.RefreshAccessToken(request.RefreshToken);
                if (result == null)
                {
                    _logger.LogWarning("Invalid or expired refresh token");
                    return Unauthorized("Invalid or expired refresh token."); }
                _logger.LogInformation("Access token refreshed successfully");
                return Ok(result); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing access token");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
