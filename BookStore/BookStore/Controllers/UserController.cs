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




        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserModel userModel)
        {
            try
            {
                _userService.RegisterUser(userModel);
                return Ok(new { message = "User registered successfully." });
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
                var token = _userService.ValidateUser(userLoginModel);

                if (token == null)
                {
                    
                    return Unauthorized(new { Error = "Unauthorized: Invalid email or password." });
                }

               
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult getUserById(int id) {
            try
            {

                var user = _userService.getUserById(id);

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
                var user = _userService.getUserById(id);
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


        [HttpGet("getall")]
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

        [HttpPut("update/{id}")]
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
                var result = _userService.ResetPassword(token, model.NewPassword);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
               
                return BadRequest(new { Message = ex.Message });
            }
        }



    }

}

