using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeSync.Core.DTO.BusinessDTO;
using TimeSync.Core.DTO.functionalDTO;
using TimeSync.Services;

namespace TimeSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _services;

        public UserController(UserService services)
        {
            _services = services;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            var result = await _services.CreateUserAsync(userDTO);

            if (result == null)
            {
                return BadRequest("User already exists");
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] loginDTO userDTO)
        {
            var result = await _services.LoginAsync(userDTO.Email, userDTO.Password);

            if (result == null)
            {
                return Unauthorized("Invalid login credentials or account is disabled/locked.");
            }

            return Ok(result);
        }
    }
}
