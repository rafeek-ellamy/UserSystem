using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserSystem.Data;

namespace UserSystem.Repositories.RolesRepository
{
    public class RolesRepository(UserSystemDbContext dbContext) : IRolesRepository
    {
        private readonly UserSystemDbContext _dbContext = dbContext;

        public IQueryable<IdentityRole<string>> GetAll()
        {
            return dbContext.Roles.AsNoTracking();
        }
    }
}
