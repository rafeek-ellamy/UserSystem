using Configurations.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace UserSystem.Repositories.UserRolesRepository
{
    public interface IUserRolesRepository : IScopedRepository
    {
        IQueryable<IdentityUserRole<string>> GetAll();
    }
}
