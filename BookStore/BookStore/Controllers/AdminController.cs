using BuisnessLayer.Interface;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
