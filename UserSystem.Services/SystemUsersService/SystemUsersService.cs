using Configurations.Extensions;
using Configurations.GenericApiResponse;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using UserSystem.Data.Entities;
using UserSystem.Repositories.RolesRepository;
using UserSystem.Repositories.UserRepository;
using UserSystem.Repositories.UserRolesRepository;
using UserSystem.Services.AuditLogService;
using UserSystem.Services.Localization;
using UserSystem.Services.Models.Input;
using UserSystem.Services.Models.Output;
using UserSystem.Services.UtilityHelper;

namespace UserSystem.Services.SystemUsersService
{
    public class SystemUsersService : BaseLogger, ISystemUsersService
    {
        #region Ctor
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly UserManager<UserProfile> _userManager;
        private readonly IUserRepository _userRepo;
        private readonly IUtilityHelper _utilityHelper;
        private readonly ILogger<SystemUsersService> _logger;
        private readonly IUserRolesRepository _userRolesRepo;
        private readonly IAuditLogService _auditLogService;
        private readonly IRolesRepository _rolesRepo;

        public SystemUsersService(UserManager<UserProfile> userManager,
            IUserRepository userRepo,
            IUtilityHelper utilityHelper,
            ILogger<SystemUsersService> logger,
            IUserRolesRepository userRolesRepo,
            IAuditLogService auditLogService,
            IRolesRepository rolesRepo,
            IStringLocalizer<Messages> localizer) : base(logger)
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _utilityHelper = utilityHelper;
            _logger = logger;
            _userRolesRepo = userRolesRepo;
            _auditLogService = auditLogService;
            _rolesRepo = rolesRepo;
            _localizer = localizer;
        }
        #endregion

        #region Methode
        public async Task<PagedApiResponse<List<SystemUsersOutputDto>>> GetSystemUsers(SystemUserFilterInputDto input)
        {
            var usersQuery = _userRepo.GetAll()
                .WhereIf(!string.IsNullOrEmpty(input.Name),
                    a => EF.Functions.Like(a.FirstName, $"%{input.Name}%") ||
                         EF.Functions.Like(a.LastName, $"%{input.Name}%"))
                .WhereIf(!string.IsNullOrEmpty(input.UserName), a => EF.Functions.Like(a.UserName, $"%{input.UserName}%"))
                .WhereIf(!string.IsNullOrEmpty(input.Email), a => EF.Functions.Like(a.Email, $"%{input.Email}%"));

            _logger.LogInformation("Filtered user count before role assignment: {Count}", usersQuery.Count());

            var filterData = (from user in usersQuery
                              join userRole in _userRolesRepo.GetAll() on user.Id equals userRole.UserId
                              join role in _rolesRepo.GetAll() on userRole.RoleId equals role.Id
                              where string.IsNullOrEmpty(input.RoleId) || role.Id == input.RoleId
                              group role by user into userGroup
                              select new
                              {
                                  User = userGroup.Key,
                                  Roles = userGroup.Select(r => new { r.Id, r.Name }).ToList()
                              });


            var totalCount = filterData.Count();
            _logger.LogInformation("Total users found after role filtering: {TotalCount}", totalCount);
            var paginatedData = await filterData.Skip(input.PageSize * (input.PageIndex - 1)).Take(input.PageSize).ToListAsync();
            _logger.LogInformation("Fetching paginated users - Retrieved: {RetrievedCount}", paginatedData.Count);

            var resData = paginatedData.Select(a => new SystemUsersOutputDto()
            {
                UserId = a.User.Id,
                FirstName = a.User.FirstName,
                LastName = a.User.LastName,
                FullName = a.User.FullName,
                CreateAt = a.User.CreatedAt,
                UpdateAt = a.User.UpdatedAt,
                UserName = a.User.UserName ?? string.Empty,
                Email = a.User.Email ?? string.Empty,
                Roles = a.Roles.Select(r => new LookupsOutputDto<string>()
                {
                    Id = r.Id,
                    NameEn = r.Name
                }).ToList(),
            }).ToList();

            _logger.LogInformation("Returning {UserCount} users in response.", resData.Count);

            return new PagedApiResponse<List<SystemUsersOutputDto>>(true, resData, totalCount, input.PageIndex, input.PageSize);
        }

        public async Task<ApiResponse<SystemUsersOutputDto>> GetSystemUserById(string userId)
        {
            _logger.LogInformation("Fetching user details for UserId: {UserId}", userId);

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found for UserId: {UserId}", userId);
                return new ApiResponse<SystemUsersOutputDto>(false, _localizer["UserNotFound"]);
            }

            _logger.LogInformation("User found: {UserName} (ID: {UserId})", user.UserName, user.Id);

            var userRoles = await (from userRole in _userRolesRepo.GetAll()
                                   join role in _rolesRepo.GetAll() on userRole.RoleId equals role.Id
                                   where userRole.UserId == userId
                                   select new LookupsOutputDto<string>
                                   {
                                       Id = role.Id,
                                       NameEn = role.Name
                                   }).ToListAsync();

            _logger.LogInformation("Fetched {RoleCount} roles for UserId: {UserId}", userRoles.Count, userId);

            var result = new SystemUsersOutputDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = userRoles
            };

            _logger.LogInformation("Returning user data for UserId: {UserId}", userId);
            return new ApiResponse<SystemUsersOutputDto>(result);
        }

        public async Task<ApiResponse<bool>> CreateSystemUsers(CreateSystemUserInputDto input)
        {
            _logger.LogInformation("----------Starting Create System Users for UserName: {UserName}, Email: {Email}", input.UserName, input.Email);

            var isUsernameUsed = await _userManager.FindByNameAsync(input.UserName);
            if (isUsernameUsed is not null)
            {
                _logger.LogWarning("Creation failed: Username '{UserName}' is already in use.", input.UserName);
                return new ApiResponse<bool>(false, _localizer["UserNameAlreadyExist"]);
            }

            var isEmailUsed = await _userManager.FindByEmailAsync(input.Email);
            if (isEmailUsed is not null)
            {
                _logger.LogWarning("Creation failed: Email '{Email}' is already in use.", input.Email);
                return new ApiResponse<bool>(false, _localizer["EmailAlreadyExist"]);
            }

            var user = new UserProfile()
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                UserName = input.UserName,
                CreatedAt = DateTime.UtcNow
            };

            var addRes = await _userManager.CreateAsync(user, input.Password);
            if (!addRes.Succeeded)
            {
                var errorMessage = addRes.Errors.First().Description;
                _logger.LogError("User creation failed for {UserName}: {ErrorMessage}", input.UserName, errorMessage);
                return new ApiResponse<bool>(false, errorMessage);
            }

            await _auditLogService.AuditCreate("User", user.Id, input.CurrentUserId);
            _logger.LogInformation("User '{UserName}' created successfully.", user.UserName);

            // Assign Role
            var roleResult = await _userManager.AddToRolesAsync(user, input.Roles);
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Failed to assign roles to the User '{UserName}'", user.UserName);
            }
            else
            {
                _logger.LogInformation("Roles assigned to the User '{UserName}'", user.UserName);
            }

            _logger.LogInformation("User '{UserName}' registered successfully.", user.UserName);

            return new ApiResponse<bool>(true);
        }

        public async Task<ApiResponse<bool>> UpdateSystemUsers(UpdateSystemUserInputDto input)
        {
            _logger.LogInformation("---------- Starting Update System User for UserId: {UserId}", input.UserId);

            var user = await _userManager.FindByIdAsync(input.UserId);
            if (user is null)
            {
                _logger.LogWarning("Update failed: User with ID '{UserId}' not found.", input.UserId);
                return new ApiResponse<bool>(false, _localizer["UserNotFound"]);
            }

            // Check if the username is already in use by another user
            var existingUserByUsername = await _userManager.FindByNameAsync(input.UserName);
            if (existingUserByUsername != null && existingUserByUsername.Id != input.UserId)
            {
                _logger.LogWarning("Update failed: Username '{UserName}' is already in use.", input.UserName);
                return new ApiResponse<bool>(false, _localizer["UserNameAlreadyExist"]);
            }

            // Check if the email is already in use by another user
            var existingUserByEmail = await _userManager.FindByEmailAsync(input.Email);
            if (existingUserByEmail != null && existingUserByEmail.Id != input.UserId)
            {
                _logger.LogWarning("Update failed: Email '{Email}' is already in use.", input.Email);
                return new ApiResponse<bool>(false, _localizer["EmailAlreadyExist"]);
            }

            var oldUser = new UserProfile(user)
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            // Update user details
            user.FirstName = input.FirstName;
            user.LastName = input.LastName;
            user.Email = input.Email;
            user.UserName = input.UserName;
            user.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errorMessage = updateResult.Errors.First().Description;
                _logger.LogError("User update failed for {UserId}: {ErrorMessage}", input.UserId, errorMessage);
                return new ApiResponse<bool>(false, errorMessage);
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(input.Password))
            {
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordChangeResult = await _userManager.ResetPasswordAsync(user, passwordResetToken, input.Password);
                if (!passwordChangeResult.Succeeded)
                {
                    _logger.LogError("Failed to update password for User '{UserId}'", input.UserId);
                    return new ApiResponse<bool>(false, _localizer["PasswordUpdateFailed"]);
                }
            }

            // Update roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(input.Roles).ToList();
            var rolesToAdd = input.Roles.Except(currentRoles).ToList();

            if (rolesToRemove.Any())
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeRolesResult.Succeeded)
                {
                    _logger.LogError("Failed to remove roles from User '{UserId}'", input.UserId);
                }
            }

            if (rolesToAdd.Any())
            {
                var addRolesResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addRolesResult.Succeeded)
                {
                    _logger.LogError("Failed to add roles to User '{UserId}'", input.UserId);
                }
            }

            await _auditLogService.AuditUpdate("User", user.Id,
                input.CurrentUserId, JsonSerializer.Serialize(oldUser),
                JsonSerializer.Serialize(user));
            _logger.LogInformation("User '{UserId}' updated successfully.", input.UserId);
            return new ApiResponse<bool>(true);
        }

        public async Task<ApiResponse<bool>> DeleteSystemUsers(string userId, string currentUserId)
        {
            _logger.LogInformation("---------- Starting Delete System User for UserId: {UserId} by CurrentUser: {CurrentUserId}", userId, currentUserId);

            // Prevent a user from deleting his account
            if (userId == currentUserId)
            {
                _logger.LogWarning("Delete operation failed: User '{UserId}' attempted to delete themselves.", userId);
                return new ApiResponse<bool>(false, _localizer["CannotDeleteYourself"]);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                _logger.LogWarning("Delete failed: User with ID '{UserId}' not found.", userId);
                return new ApiResponse<bool>(false, _localizer["UserNotFound"]);
            }

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                var errorMessage = deleteResult.Errors.First().Description;
                _logger.LogError("User deletion failed for {UserId}: {ErrorMessage}", userId, errorMessage);
                return new ApiResponse<bool>(false, errorMessage);
            }

            _userRepo.Remove(user);
            await _auditLogService.AuditDelete("User", userId, currentUserId);
            _logger.LogInformation("User '{UserId}' deleted successfully.", userId);
            return new ApiResponse<bool>(true);
        }
        #endregion
    }
}
