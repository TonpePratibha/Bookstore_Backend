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




        public UserController(IUserService userService)   // camalCase fro parameters
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Register([FromBody] UserModel userModel)
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
               
                var newUser = _userService.RegisterUser(userModel);          //camalcase for variable
                return Ok( new { message = "User registered successfully." , user=newUser });  

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLoginModel)
        {
            try
            {
                var response = _userService.ValidateUser(userLoginModel);

                if (response == null)
                {
                    
                    return Unauthorized(new { Error = "Unauthorized: Invalid email or password." });
                }

               
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUserById(int id) {     //pascalCase fro methodname
            try
            {

                var user = _userService.GetUserById(id);

                if (user == null)
                {
                    return NotFound(new { error = "user not found" });
                }

                return Ok(new { user });
            }
            catch (Exception ex) {
                return BadRequest(new { error=ex.Message});
            }
}



        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _userService.GetUserById(id);
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                _userService.Deleteuser(id);
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userService.GetAllUsers();
                if (users == null || !users.Any())
                {
                    return NotFound(new { message = "No users found" });
              }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel model)
        {
            try
            {
                _userService.UpdateUser(id, model);
                return Ok(new { Message = "User updated successfully" });
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
                
                _userService.SendResetPasswordEmail(model.Email);
       

               // return Ok("Password reset email sent.");
                return Ok(new { message = "Password reset email sent." });
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
                var result = _userService.ResetPassword(token, model.NewPassword);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
               
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("accesslogin")]
        public IActionResult AccessLogin(UserLogin login)
        {
            try
            {
                var result = _userService.AcesstokenLogin(login);
                if (result == null) return Unauthorized("Invalid credentials.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during login: {ex.Message}");
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(RefreshRequest request)
        {
            try
            {
                var result = _userService.RefreshAccessToken(request.RefreshToken);
                if (result == null) return Unauthorized("Invalid or expired refresh token.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while refreshing token: {ex.Message}");
            }
        }



    }

}

