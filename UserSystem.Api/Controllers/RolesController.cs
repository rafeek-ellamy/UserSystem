using Configurations.GenericApiResponse;
using Configurations.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserSystem.Services.Models.Output;
using UserSystem.Services.RoleService;

namespace UserSystem.Api.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        #region Ctor
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        #endregion

        #region Endpoints   
        [HttpGet("get-all")]
        [Authorize(Roles = AuthorizationPolicies.SuperAdmin)]
        [ProducesResponseType(typeof(ApiResponse<List<LookupsOutputDto<string>>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _roleService.GetAllRoles();

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("get-user-roles")]
        [Authorize(Roles = AuthorizationPolicies.SuperAdmin)]
        [ProducesResponseType(typeof(ApiResponse<List<LookupsOutputDto<string>>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUserRoles([FromQuery][Required] string userId)
        {
            var response = await _roleService.GetAllUserRoles(userId);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }
        #endregion
    }
}
