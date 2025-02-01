using Configurations.GenericApiResponse;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using UserSystem.Data.Entities;
using UserSystem.Services.Localization;
using UserSystem.Services.Models.Input;
using UserSystem.Services.Models.Output;
using UserSystem.Services.UtilityHelper;

namespace UserSystem.Services.AuthService
{
    public class AuthService : BaseLogger, IAuthService
    {
        #region Ctor
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly UserManager<UserProfile> _userManager;
        private readonly IUtilityHelper _utilityHelper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<UserProfile> userManager,
            IUtilityHelper utilityHelper,
            ILogger<AuthService> logger,
            IStringLocalizer<Messages> localizer) : base(logger)
        {
            _userManager = userManager;
            _utilityHelper = utilityHelper;
            _logger = logger;
            _localizer = localizer;
        }
        #endregion

        #region Methode
        public async Task<ApiResponse<UserLoginOutputDto>> RegisterSystemUser(RegisterSystemUserInputDto input)
        {
            _logger.LogInformation("----------Starting user registration for UserName: {UserName}, Email: {Email}", input.UserName, input.Email);

            var isUsernameUsed = await _userManager.FindByNameAsync(input.UserName);
            if (isUsernameUsed is not null)
            {
                _logger.LogWarning("Registration failed: Username '{UserName}' is already in use.", input.UserName);
                return new ApiResponse<UserLoginOutputDto>(false, _localizer["UserNameAlreadyExist"]);
            }

            var isEmailUsed = await _userManager.FindByEmailAsync(input.Email);
            if (isEmailUsed is not null)
            {
                _logger.LogWarning("Registration failed: Email '{Email}' is already in use.", input.Email);
                return new ApiResponse<UserLoginOutputDto>(false, _localizer["EmailAlreadyExist"]);
            }

            var user = new UserProfile()
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                UserName = input.UserName,
            };

            var addRes = await _userManager.CreateAsync(user, input.Password);
            if (!addRes.Succeeded)
            {
                var errorMessage = addRes.Errors.First().Description;
                _logger.LogError("User creation failed for {UserName}: {ErrorMessage}", input.UserName, errorMessage);
                return new ApiResponse<UserLoginOutputDto>(false, errorMessage);
            }

            _logger.LogInformation("User '{UserName}' created successfully.", user.UserName);

            // Assign Role
            var roleResult = await _userManager.AddToRoleAsync(user, "SystemUser");
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Failed to assign role 'SystemUser' to User '{UserName}'", user.UserName);
            }
            else
            {
                _logger.LogInformation("Role 'SystemUser' assigned to User '{UserName}'", user.UserName);
            }

            // Generate Token
            var token = await _utilityHelper.GenerateSecurityToken(user);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Failed to generate token for User '{UserName}'", user.UserName);
                return new ApiResponse<UserLoginOutputDto>(false, "Token generation failed.");
            }

            _logger.LogInformation("User '{UserName}' registered successfully.", user.UserName);

            var userLoginRes = new UserLoginOutputDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            };

            return new ApiResponse<UserLoginOutputDto>(userLoginRes);
        }

        public async Task<ApiResponse<UserLoginOutputDto>> Login(LoginInputDto input)
        {
            _logger.LogInformation("----------Starting user Login for UserName: {UserName}", input.UserName);

            var user = await _userManager.FindByNameAsync(input.UserName);
            if (user is null || !await _userManager.CheckPasswordAsync(user, input.Password))
            {
                _logger.LogWarning("Login failed: Username '{UserName}' or password is incorrect.", input.UserName);
                return new ApiResponse<UserLoginOutputDto>(false, _localizer["UserNotExist"]);
            }

            // Generate Token
            var token = await _utilityHelper.GenerateSecurityToken(user);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Failed to generate token for User '{UserName}'", user.UserName);
                return new ApiResponse<UserLoginOutputDto>(false, "Token generation failed.");
            }

            _logger.LogInformation("User '{UserName}' Login successfully.", user.UserName);

            var userLoginRes = new UserLoginOutputDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            };

            return new ApiResponse<UserLoginOutputDto>(userLoginRes);
        }

        #endregion
    }
}
