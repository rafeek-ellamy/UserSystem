using Configurations.GenericApiResponse;
using Configurations.Interfaces;
using UserSystem.Services.Models.Output;

namespace UserSystem.Services.RoleService
{
    public interface IRoleService : IScopedService
    {
        Task<ApiResponse<List<LookupsOutputDto<string>>>> GetAllRoles();
        Task<ApiResponse<List<LookupsOutputDto<string>>>> GetAllUserRoles(string userId);

    }
}
