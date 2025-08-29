using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Habitus.Domain.Entities;
using Habitus.Domain.Enums;

namespace Habitus.Domain.Interfaces.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<T>> GetRecentAsync(int count);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Remove(T entity);
        Task SaveChangesAsync();
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
        Task<int> GetActiveUsersCountAsync();
        Task<int> GetInactiveUsersCountAsync();
        Task<IEnumerable<User>> GetUsersCreatedAfterAsync(DateTime date);
        Task<User?> GetUserWithRefreshTokensAsync(int userId);
    }

    /*
    // Temporariamente comentado devido a conflitos de namespace
    public interface ITaskRepository : IRepository<Entities.Task>
    {
        Task<IEnumerable<Entities.Task>> GetTasksByUserIdAsync(int userId);
        Task<IEnumerable<Entities.Task>> GetTasksByCategoryAsync(int userId, int categoryId);
        Task<IEnumerable<Entities.Task>> GetTasksByStatusAsync(int userId, TaskStatus status);
        Task<IEnumerable<Entities.Task>> GetOverdueTasksAsync(int userId);
        Task<IEnumerable<Entities.Task>> GetTasksDueTodayAsync(int userId);
        Task<IEnumerable<Entities.Task>> GetCompletedTasksAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    }
    */

    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesByUserIdAsync(int userId);
        Task<bool> CategoryExistsAsync(int userId, string name);
        Task<int> GetTasksCountByCategoryAsync(int categoryId);
    }

    public interface IHabitRepository : IRepository<Habit>
    {
        Task<IEnumerable<Habit>> GetHabitsByUserIdAsync(int userId);
        Task<IEnumerable<Habit>> GetActiveHabitsAsync(int userId);
        Task<bool> HabitExistsAsync(int userId, string name);
    }

    public interface IHabitLogRepository : IRepository<HabitLog>
    {
        Task<IEnumerable<HabitLog>> GetLogsByHabitIdAsync(int habitId, DateTime? startDate = null, DateTime? endDate = null);
        Task<HabitLog?> GetLogByDateAsync(int habitId, DateTime date);
        Task<IEnumerable<HabitLog>> GetUserLogsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetStreakAsync(int habitId);
    }

    public interface ICalendarEventRepository : IRepository<CalendarEvent>
    {
        Task<IEnumerable<CalendarEvent>> GetEventsByUserIdAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<CalendarEvent>> GetUpcomingEventsAsync(int userId, int days = 7);
        Task<IEnumerable<CalendarEvent>> GetEventsByDateAsync(int userId, DateTime date);
        Task<bool> HasConflictingEventsAsync(int userId, DateTime startTime, DateTime endTime, int? excludeEventId = null);
    }

    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(int userId);
        Task RevokeAllUserTokensAsync(int userId);
        Task<bool> IsTokenValidAsync(string token);
    }

    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetLogsByUserIdAsync(int userId, int page = 1, int pageSize = 20);
        Task<IEnumerable<AuditLog>> GetSystemLogsAsync(int page = 1, int pageSize = 20);
        Task<IEnumerable<AuditLog>> GetLogsByActionAsync(string action, int page = 1, int pageSize = 20);
        Task<IEnumerable<AuditLog>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 20);
    }
}
