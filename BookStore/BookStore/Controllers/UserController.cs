using BuisnessLayer.Interface;
using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/user")]
    [ApiController]
   
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        private readonly IUserRepository _userRepository;

        private readonly ILogger<UserController> _logger;


        public UserController(IUserService userService, ILogger<UserController> logger)   // camalCase fro parameters
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Register([FromBody] UserModel userModel)
        {
            _logger.LogInformation("Register endpoint called.");
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
                    _logger.LogWarning("User registration validation failed.");
                    return BadRequest(new { message = "Validation failed", errors });
                } 
               
                var newUser = _userService.RegisterUser(userModel);          //camalcase for variable
                _logger.LogInformation("User registered successfully.");
                return Ok( new { message = "User registered successfully." , user=newUser });  

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration.");
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLoginModel)
        {
            _logger.LogInformation("Login endpoint called.");
            try
            {
                var response = _userService.ValidateUser(userLoginModel);

                if (response == null)
                {
                    _logger.LogWarning("Invalid login attempt.");
                    return Unauthorized(new { Error = "Unauthorized: Invalid email or password." });
                }

                _logger.LogInformation("User logged in successfully.");
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login.");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUserById(int id) {//pascalCase fro methodname
            _logger.LogInformation($"GetUserById called with ID: {id}");
            try
            {

                var user = _userService.GetUserById(id);

                if (user == null)
                {
                    _logger.LogWarning("User not found.");
                    return NotFound(new { error = "user not found" });
                }

                return Ok(new { user });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error fetching user by ID.");
                return BadRequest(new { error=ex.Message});
            }
}



        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            _logger.LogInformation($"DeleteUser called with ID: {id}");
            try
            {
                var user = _userService.GetUserById(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found for deletion.");
                    return NotFound(new { error = "User not found" });
                }

                _userService.Deleteuser(id);
                _logger.LogInformation("User deleted successfully.");
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user.");
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult GetAllUsers()
        {
            _logger.LogInformation("GetAllUsers endpoint called.");
            try
            {
                var users = _userService.GetAllUsers();
                if (users == null || !users.Any())
                {
                    _logger.LogInformation("No users found.");
                    return NotFound(new { message = "No users found" });
              }
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel model)
        {
            _logger.LogInformation($"UpdateUser called with ID: {id}");
            try
            {
                _userService.UpdateUser(id, model);
                return Ok(new { Message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user.");
                return BadRequest(new { Message = ex.Message });
            }
        }



        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPaswordModel model)
        {
            _logger.LogInformation("ForgotPassword endpoint called.");
            if (string.IsNullOrEmpty(model.Email))
            {
                _logger.LogWarning("Forgot password request without email.");
                return BadRequest("Email is required.");
            }

            try
            {
                
                _userService.SendResetPasswordEmail(model.Email);
       

               // return Ok("Password reset email sent.");
                return Ok(new { message = "Password reset email sent." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reset password email.");
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {

            _logger.LogInformation("ResetPassword endpoint called.");
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogWarning("Missing or invalid authorization header for password reset.");
                return BadRequest(new { Message = "Authorization header is missing or invalid" });
            }

            var token = authHeader.Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Reset password token missing.");
                return BadRequest(new { Message = "Token is required" });
            }

            try
            {
                var result = _userService.ResetPassword(token, model.NewPassword);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password.");
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("accesslogin")]
        public IActionResult AccessLogin(UserLogin login)
        {
            _logger.LogInformation("AccessLogin endpoint called.");
            try
            {
                var result = _userService.AcesstokenLogin(login);
                if (result == null) return Unauthorized("Invalid credentials.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during access token login.");
                return StatusCode(500, $"An error occurred during login: {ex.Message}");
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(RefreshRequest request)
        {
            _logger.LogInformation("Refresh token endpoint called.");
            try
            {
                var result = _userService.RefreshAccessToken(request.RefreshToken);
                if (result == null) return Unauthorized("Invalid or expired refresh token.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token.");
                return StatusCode(500, $"An error occurred while refreshing token: {ex.Message}");
            }
        }



    }

}

