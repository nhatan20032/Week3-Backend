using EFCorePracticeAPI.Dtos;
using EFCorePracticeAPI.Service.Interface;
using EFCorePracticeAPI.ViewModals.User;
using Microsoft.AspNetCore.Mvc;

namespace EFCorePracticeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        protected readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetAllUser(int page = 1, int pageSize = 10, string search = "")
        {
            var result = await _userService.GetAllUser(page, pageSize, search);
            return Ok(result);
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            var result = await _userService.GetUserById(id);
            if (result == null)
            {
                return NotFound($"Entity of type User with ID {id} not found.");
            }
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await _userService.Login(loginRequest.Username, loginRequest.Password);
            if (result == null)
            {
                return NotFound($"Wrong Username or Password please check");
            }
            return Ok(result);
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] V_User user)
        {
            var result = await _userService.AddUser(user);
            if (result == null)
            {
                return NotFound($"Entity of type User with ID {user.Id} not found.");
            }
            return Ok(result);
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] V_User user)
        {
            var result = await _userService.UpdateUser(user);
            if (result == null)
            {
                return NotFound($"Entity of type User with ID {user.Id} not found.");
            }
            return Ok(result);
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var result = await _userService.DeleteUser(id);
            if (result == null)
            {
                return NotFound($"Entity of type User with ID {id} not found.");
            }
            return Ok(result);
        }
    }
}
