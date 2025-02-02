using Configurations.GenericApiResponse;
using Configurations.Interfaces;
using UserSystem.Services.Models.Input;
using UserSystem.Services.Models.Output;

namespace UserSystem.Services.SystemUsersService
{
    public interface ISystemUsersService : IScopedService
    {
        Task<PagedApiResponse<List<SystemUsersOutputDto>>> GetSystemUsers(SystemUserFilterInputDto filterDto);
        Task<ApiResponse<SystemUsersOutputDto>> GetSystemUserById(string userId);

        Task<ApiResponse<bool>> CreateSystemUsers(CreateSystemUserInputDto input);

        Task<ApiResponse<bool>> UpdateSystemUsers(UpdateSystemUserInputDto input);
        Task<ApiResponse<bool>> DeleteSystemUsers(string userId, string currentUserId);
    }
}
