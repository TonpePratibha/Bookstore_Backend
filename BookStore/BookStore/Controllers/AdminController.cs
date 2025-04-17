using BuisnessLayer.Interface;
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

        [HttpPost("register")]
        public IActionResult Register([FromBody] AdminModel adminModel)
        {
            try
            {
                _adminService.RegisterAdmin(adminModel);
                return Ok(new { message = "admin registered successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminLogin adminLoginModel) {


            try {

                var token = _adminService.ValidateAdmin(adminLoginModel);



                if (token == null)
                {
                    return Unauthorized(new { Error = "unauthorized invalid email or password" });
                }
                return Ok(new { Token = token });


            }
            catch (Exception ex) {
                return BadRequest(new { Error = ex.Message });

            }





        }
        [HttpGet("{id}")]
        [Authorize]
            public IActionResult getAdminById(int id) {
            try
            {
                var admin = _adminService.getAdminById(id);
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


    } }
