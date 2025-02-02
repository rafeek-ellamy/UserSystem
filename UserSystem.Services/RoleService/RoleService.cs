using Configurations.GenericApiResponse;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using UserSystem.Data.Entities;
using UserSystem.Repositories.RolesRepository;
using UserSystem.Repositories.UserRepository;
using UserSystem.Repositories.UserRolesRepository;
using UserSystem.Services.Localization;
using UserSystem.Services.Models.Output;
using UserSystem.Services.UtilityHelper;

namespace UserSystem.Services.RoleService
{
    public class RoleService : BaseLogger, IRoleService
    {
        #region Ctor
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly UserManager<UserProfile> _userManager;
        private readonly IUserRepository _userRepo;
        private readonly IUtilityHelper _utilityHelper;
        private readonly ILogger<RoleService> _logger;
        private readonly IUserRolesRepository _userRolesRepo;
        private readonly IRolesRepository _rolesRepo;

        public RoleService(UserManager<UserProfile> userManager,
            IUserRepository userRepo,
            ILogger<RoleService> logger,
            IUserRolesRepository userRolesRepo,
            IRolesRepository rolesRepo,
            IStringLocalizer<Messages> localizer) : base(logger)
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _logger = logger;
            _userRolesRepo = userRolesRepo;
            _rolesRepo = rolesRepo;
            _localizer = localizer;
        }
        #endregion

        #region Methode
        public async Task<ApiResponse<List<LookupsOutputDto<string>>>> GetAllRoles()
        {
            var userRoles = _rolesRepo.GetAll();

            var result = await userRoles.Select(one => new LookupsOutputDto<string>
            {
                Id = one.Id,
                NameEn = one.Name ?? string.Empty,
                NameAr = string.Empty,
            }).ToListAsync();

            return new ApiResponse<List<LookupsOutputDto<string>>>(result);
        }

        public async Task<ApiResponse<List<LookupsOutputDto<string>>>> GetAllUserRoles(string userId)
        {
            var userRoles = _userRolesRepo.GetAll().Where(a => a.UserId == userId);

            var result = await userRoles.Select(one => new LookupsOutputDto<string>
            {
                Id = one.RoleId,
                NameEn = string.Empty,
                NameAr = string.Empty,
            }).ToListAsync();

            return new ApiResponse<List<LookupsOutputDto<string>>>(result);
        }
        #endregion
    }
}
