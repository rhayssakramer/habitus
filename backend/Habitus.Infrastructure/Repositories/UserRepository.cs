using Habitus.Domain.Entities;
using Habitus.Domain.Interfaces;
using Habitus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Habitus.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(HabitusDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email.ToLower() && !u.IsDeleted);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Username == username.ToLower() && !u.IsDeleted);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet
                .AnyAsync(u => u.Email == email.ToLower() && !u.IsDeleted);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbSet
                .AnyAsync(u => u.Username == username.ToLower() && !u.IsDeleted);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Where(u => u.IsActive && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(Domain.Enums.UserRole role)
        {
            return await _dbSet
                .Where(u => u.Role == role && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted && (
                    u.FirstName.Contains(searchTerm) ||
                    u.LastName.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm) ||
                    u.Username.Contains(searchTerm)
                ))
                .ToListAsync();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _dbSet
                .CountAsync(u => u.IsActive && !u.IsDeleted);
        }

        public async Task<int> GetInactiveUsersCountAsync()
        {
            return await _dbSet
                .CountAsync(u => !u.IsActive && !u.IsDeleted);
        }

        public async Task<IEnumerable<User>> GetUsersCreatedAfterAsync(DateTime date)
        {
            return await _dbSet
                .Where(u => u.CreatedAt >= date && !u.IsDeleted)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithRefreshTokensAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }
    }
}
