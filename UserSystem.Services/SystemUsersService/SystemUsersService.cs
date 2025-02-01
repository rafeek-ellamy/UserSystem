using Configurations.Extensions;
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
        private readonly IRolesRepository _rolesRepo;

        public SystemUsersService(UserManager<UserProfile> userManager,
            IUserRepository userRepo,
            IUtilityHelper utilityHelper,
            ILogger<SystemUsersService> logger,
            IUserRolesRepository userRolesRepo,
            IRolesRepository rolesRepo,
            IStringLocalizer<Messages> localizer) : base(logger)
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _utilityHelper = utilityHelper;
            _logger = logger;
            _userRolesRepo = userRolesRepo;
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

            var filterData = (from user in usersQuery
                              join userRole in _userRolesRepo.GetAll() on user.Id equals userRole.UserId
                              join role in _rolesRepo.GetAll() on userRole.RoleId equals role.Id
                              where string.IsNullOrEmpty(input.RoleId) || role.Id == input.RoleId
                              select new { user, role });

            var totalCount = filterData.Count();
            filterData = (filterData.Skip(input.PageSize * (input.PageIndex - 1)).Take(input.PageSize));

            var resData = await filterData.Select(a => new SystemUsersOutputDto()
            {
                UserId = a.user.Id,
                FirstName = a.user.FirstName,
                LastName = a.user.LastName,
                FullName = a.user.FullName,
                UserName = a.user.UserName ?? string.Empty,
                Email = a.user.Email ?? string.Empty,
                RoleId = a.role.Id,
                RoleName = a.role.Name ?? string.Empty,
            }).ToListAsync();

            return new PagedApiResponse<List<SystemUsersOutputDto>>(true, resData, totalCount, input.PageIndex, input.PageSize);
        }
        #endregion
    }
}
