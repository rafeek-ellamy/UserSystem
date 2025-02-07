﻿using Microsoft.EntityFrameworkCore;
using UserSystem.Data;
using UserSystem.Data.Entities;

namespace UserSystem.Repositories.UserRepository
{
    public class UserRepository(UserSystemDbContext context) : IUserRepository
    {
        public IQueryable<UserProfile> GetAll()
        {
            return context.Users.AsQueryable();
        }

        public async Task<UserProfile?> GetByIdAsync(string id)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task InsertAsync(UserProfile entity)
        {
            await context.Users.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task<UserProfile> UpdateAsync(UserProfile entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }

        public void Remove(UserProfile entity)
        {
            context.Users.Remove(entity);
            context.SaveChanges();
        }
    }
}
