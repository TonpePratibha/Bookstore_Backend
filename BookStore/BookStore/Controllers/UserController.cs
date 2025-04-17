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
        public IActionResult getUsrById(int id) {
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
                return BadRequest(new { error = "unauthorized user" });
            }
}



        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                 _userService.Deleteuser(id);
                return Ok("user deleted");


            }

            catch (Exception ex) { 
              return BadRequest(new { error="unauthorzed user"});
            
            }
        }
        }

    }

