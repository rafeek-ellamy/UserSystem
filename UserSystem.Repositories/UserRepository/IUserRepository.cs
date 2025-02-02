using Configurations.Interfaces;
using UserSystem.Data.Entities;

namespace UserSystem.Repositories.UserRepository
{
    public interface IUserRepository : IScopedRepository
    {
        IQueryable<UserProfile> GetAll();
        Task<UserProfile?> GetByIdAsync(string id);
        Task InsertAsync(UserProfile user);
        Task<UserProfile> UpdateAsync(UserProfile user);
        void Remove(UserProfile entity);
    }
}
