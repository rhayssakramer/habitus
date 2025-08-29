using System;
using System.Threading.Tasks;
using Habitus.Domain.Entities;
using Habitus.Domain.Enums;

namespace Habitus.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
        Task<User?> GetByEmailConfirmationTokenAsync(string token);
        Task<User?> GetByPasswordResetTokenAsync(string token);
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
        Task<IEnumerable<User>> GetActiveUsersAsync();
    }
    
    /*
    // Temporariamente comentado devido a conflitos de namespace
    public interface ITaskRepository : IRepository<Entities.Task>
    {
        Task<IEnumerable<Entities.Task>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Entities.Task>> GetOverdueTasksAsync(Guid userId);
        Task<IEnumerable<Entities.Task>> GetTasksDueTodayAsync(Guid userId);
        Task<IEnumerable<Entities.Task>> GetCompletedTasksAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    }
    */
    
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId);
    }
    
    public interface IHabitRepository : IRepository<Habit>
    {
        Task<IEnumerable<Habit>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Habit>> GetActiveHabitsAsync(Guid userId);
    }
    
    public interface IHabitLogRepository : IRepository<HabitLog>
    {
        Task<IEnumerable<HabitLog>> GetByHabitIdAsync(Guid habitId);
        Task<HabitLog?> GetByHabitAndDateAsync(Guid habitId, DateTime date);
        Task<IEnumerable<HabitLog>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    }
    
    public interface ICalendarEventRepository : IRepository<CalendarEvent>
    {
        Task<IEnumerable<CalendarEvent>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<CalendarEvent>> GetEventsByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    }
    
    public interface IEventReminderRepository : IRepository<EventReminder>
    {
        Task<IEnumerable<EventReminder>> GetPendingRemindersAsync();
        Task<IEnumerable<EventReminder>> GetByCalendarEventIdAsync(Guid calendarEventId);
    }
    
    public interface ITaskCommentRepository : IRepository<TaskComment>
    {
        Task<IEnumerable<TaskComment>> GetByTaskIdAsync(Guid taskId);
    }
    
    public interface ITaskAttachmentRepository : IRepository<TaskAttachment>
    {
        Task<IEnumerable<TaskAttachment>> GetByTaskIdAsync(Guid taskId);
    }
    
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        System.Threading.Tasks.Task<RefreshToken?> GetByTokenAsync(string token);
        System.Threading.Tasks.Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId);
        System.Threading.Tasks.Task RevokeAllUserTokensAsync(Guid userId);
        System.Threading.Tasks.Task CleanupExpiredTokensAsync();
    }
    
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        System.Threading.Tasks.Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);
        System.Threading.Tasks.Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, Guid entityId);
        System.Threading.Tasks.Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
