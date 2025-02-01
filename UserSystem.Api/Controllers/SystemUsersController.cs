using Configurations.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using UserSystem.Services.Models.Input;
using UserSystem.Services.SystemUsersService;

namespace UserSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthorizationPolicies.SuperAdmin)]
    public class SystemUsersController : ControllerBase
    {
        #region Ctor
        private readonly ISystemUsersService _systemUsersService;
        public SystemUsersController(ISystemUsersService systemUsersService)
        {
            _systemUsersService = systemUsersService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> RegisterSystemUser([FromQuery] SystemUserFilterInputDto input)
        {
            var response = await _systemUsersService.GetSystemUsers(input);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [AllowAnonymous]
        [HttpGet("debug-token")]
        public IActionResult DebugToken()
        {
            // Get token from header

            string token = Request.Headers["Authorization"];

            if (token.StartsWith("Bearer"))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }
            var handler = new JwtSecurityTokenHandler();

            // Returns all claims present in the token

            JwtSecurityToken jwt = handler.ReadJwtToken(token);

            var claims = "List of Claims: \n\n";

            foreach (var claim in jwt.Claims)
            {
                claims += $"{claim.Type}: {claim.Value}\n";
            }

            return Ok(claims);
        }
        #endregion
    }
}
