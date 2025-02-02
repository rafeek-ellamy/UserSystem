using Configurations.GenericApiResponse;
using Configurations.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserSystem.Services.Models.Input;
using UserSystem.Services.Models.Output;
using UserSystem.Services.SystemUsersService;

namespace UserSystem.Api.Controllers
{
    [Route("api/system-user")]
    [ApiController]
    public class SystemUsersController : ControllerBase
    {
        #region Ctor
        private readonly ISystemUsersService _systemUsersService;
        public SystemUsersController(ISystemUsersService systemUsersService)
        {
            _systemUsersService = systemUsersService;
        }
        #endregion

        #region Endpoints
        [HttpGet("get-all")]
        [Authorize(Roles = AuthorizationPolicies.SuperAdmin + "," + AuthorizationPolicies.Admin)]
        [ProducesResponseType(typeof(PagedApiResponse<List<SystemUsersOutputDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSystemUsers([FromQuery] SystemUserFilterInputDto input)
        {
            var response = await _systemUsersService.GetSystemUsers(input);

            if (response.Success) return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("get-by-id")]
        [Authorize(Roles = AuthorizationPolicies.SuperAdmin + "," + AuthorizationPolicies.Admin)]
        [ProducesResponseType(typeof(ApiResponse<SystemUsersOutputDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSystemUserById([FromQuery][Required] string userId)
        {
            var response = await _systemUsersService.GetSystemUserById(userId);

            if (response.Success) return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("create")]
        [Authorize(Roles = AuthorizationPolicies.SuperAdmin)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateSystemUsers([FromBody] CreateSystemUserInputDto input)
        {
            input.CurrentUserId = User.FindFirst("UserId")?.Value;
            var response = await _systemUsersService.CreateSystemUsers(input);

            if (response.Success) return Ok(response);

            return BadRequest(response);
        }

        [HttpPut("update")]
        [Authorize(Roles = AuthorizationPolicies.SuperAdmin)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSystemUsers([FromBody] UpdateSystemUserInputDto input)
        {
            input.CurrentUserId = User.FindFirst("UserId")?.Value;
            var response = await _systemUsersService.UpdateSystemUsers(input);

            if (response.Success) return Ok(response);

            return BadRequest(response);
        }

        [HttpDelete("delete")]
        [Authorize(Roles = AuthorizationPolicies.SuperAdmin)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteSystemUsers([FromQuery][Required] string userId)
        {
            var currentUserId = User.FindFirst("UserId")?.Value;
            var response = await _systemUsersService.DeleteSystemUsers(userId, currentUserId);

            if (response.Success) return Ok(response);

            return BadRequest(response);
        }
        #endregion
    }
}
