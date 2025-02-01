using Configurations.GenericApiResponse;
using Configurations.Interfaces;
using UserSystem.Services.Models.Input;
using UserSystem.Services.Models.Output;

namespace UserSystem.Services.SystemUsersService
{
    public interface ISystemUsersService : IScopedService
    {
        Task<PagedApiResponse<List<SystemUsersOutputDto>>> GetSystemUsers(SystemUserFilterInputDto filterDto);
    }
}
