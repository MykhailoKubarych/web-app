using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using WebShop.Api.Domain;

namespace WebShop.Api.DataAccess.Repos
{
    public class UserRepository : IUserRepository
    {
        private readonly ShopDbContext _context;
        private readonly DbSet<User> _users;
        
        public UserRepository(ShopDbContext context)
        {
            _context = context;
            _users = context.Users;
        }

        public async Task<List<User>> GetAsync(Expression<Func<User, bool>> expression)
        {
            Guard.Against.Null(expression, nameof(expression));
            return await _users.Where(expression).ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            Guard.Against.Null(user, nameof(user));
            await _users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

    public interface IUserRepository
    {
        Task<List<User>> GetAsync(Expression<Func<User, bool>> expression);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}