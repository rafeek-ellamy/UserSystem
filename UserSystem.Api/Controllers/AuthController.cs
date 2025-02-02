using Configurations.GenericApiResponse;
using Microsoft.AspNetCore.Mvc;
using UserSystem.Services.AuthService;
using UserSystem.Services.Models.Input;
using UserSystem.Services.Models.Output;
using UserSystem.Services.SystemUsersService;

namespace UserSystem.Api.Controllers
{
    [ApiController]
    [Route("api/auth-user")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authAdminService;
        private readonly ISystemUsersService _systemUsersService;

        public AuthController(IAuthService authAdminService, ISystemUsersService systemUsersService)
        {
            _authAdminService = authAdminService;
            _systemUsersService = systemUsersService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<UserLoginOutputDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterSystemUser([FromBody] RegisterSystemUserInputDto input)
        {
            var response = await _authAdminService.RegisterSystemUser(input);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<UserLoginOutputDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginInputDto input)
        {
            var response = await _authAdminService.Login(input);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
