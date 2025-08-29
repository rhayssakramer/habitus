using System.Threading.Tasks;

namespace Habitus.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        // ITaskRepository Tasks { get; } // Temporariamente comentado
        ICategoryRepository Categories { get; }
        IHabitRepository Habits { get; }
        IHabitLogRepository HabitLogs { get; }
        ICalendarEventRepository CalendarEvents { get; }
        IEventReminderRepository EventReminders { get; }
        // ITaskCommentRepository TaskComments { get; } // Temporariamente comentado
        // ITaskAttachmentRepository TaskAttachments { get; } // Temporariamente comentado
        IRefreshTokenRepository RefreshTokens { get; }
        IAuditLogRepository AuditLogs { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
