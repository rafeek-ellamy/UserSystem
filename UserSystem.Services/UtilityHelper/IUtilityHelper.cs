using Configurations.Interfaces;
using UserSystem.Data.Entities;

namespace UserSystem.Services.UtilityHelper
{
    public interface IUtilityHelper : IScopedService
    {
        Task<string> GenerateSecurityToken(UserProfile user);

    }
}
