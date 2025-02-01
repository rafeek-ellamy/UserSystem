using Configurations.GenericApiResponse;
using Configurations.Interfaces;
using UserSystem.Services.Models.Input;
using UserSystem.Services.Models.Output;

namespace UserSystem.Services.AuthService
{
    public interface IAuthService : IScopedService
    {
        public Task<ApiResponse<UserLoginOutputDto>> RegisterSystemUser(RegisterSystemUserInputDto input);
        public Task<ApiResponse<UserLoginOutputDto>> Login(LoginInputDto input);
    }
}
