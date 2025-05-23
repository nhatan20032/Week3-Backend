using EFCorePracticeAPI.Service.Implement;
using EFCorePracticeAPI.Service.Interface;
using EFCorePracticeAPI.ViewModals;
using EFCorePracticeAPI.ViewModals.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFCorePracticeAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        protected readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("GetRole")]
        public async Task<IActionResult> GetAll([FromQuery] SearchDto searchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _roleService.GetAllRole(searchDto);

            return Ok(result);
        }

        [HttpGet("GetRoleById/{id}")]
        public async Task<IActionResult> GetRoleById([FromRoute] int id)
        {
            var result = await _roleService.GetRoleById(id);

            if (result == null)
            {
                return NotFound($"Entity of type Role with ID {id} not found.");
            }

            return Ok(result);
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole([FromBody] V_Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _roleService.AddRole(role);

            if (result == null)
            {
                return NotFound($"Entity of type Role with ID {role.Id} not found.");
            }

            return Ok(result);
        }

        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateUser([FromBody] V_Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _roleService.UpdateRole(role);

            if (result == null)
            {
                return NotFound($"Entity of type Role with ID {role.Id} not found.");
            }
            return Ok(result);
        }

        [HttpDelete("DeleteRole/{id}")]
        public async Task<IActionResult> DeleteRole([FromRoute] int id)
        {
            var result = await _roleService.DeleteRole(id);
            if (result == null)
            {
                return NotFound($"Entity of type Role with ID {id} not found.");
            }
            return Ok(result);
        }
    }
}
