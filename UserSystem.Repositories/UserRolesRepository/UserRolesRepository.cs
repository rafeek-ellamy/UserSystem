using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserSystem.Data;

namespace UserSystem.Repositories.UserRolesRepository
{
    public class UserRolesRepository(UserSystemDbContext dbContext) : IUserRolesRepository
    {
        private readonly UserSystemDbContext _dbContext = dbContext;

        public IQueryable<IdentityUserRole<string>> GetAll()
        {
            return dbContext.UserRoles.AsNoTracking();
        }
    }
}
