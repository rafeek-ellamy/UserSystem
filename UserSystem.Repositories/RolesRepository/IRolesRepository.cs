using Configurations.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace UserSystem.Repositories.RolesRepository
{
    public interface IRolesRepository : IScopedRepository
    {
        IQueryable<IdentityRole<string>> GetAll();
    }
}
