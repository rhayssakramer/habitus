using System;
using System.Collections.Generic;
using Habitus.Domain.Enums;

namespace Habitus.Domain.Entities
{
    public class HabitusTask : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Todo;
        public bool IsRecurring { get; set; } = false;
        public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.None;
        public int? RecurrenceInterval { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
        public string? Tags { get; set; } // JSON array of tags
        public int EstimatedMinutes { get; set; } = 0;
        public int ActualMinutes { get; set; } = 0;
        public Guid UserId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ParentTaskId { get; set; }
        
        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Category? Category { get; set; }
        public virtual Task? ParentTask { get; set; }
        public virtual ICollection<HabitusTask> SubTasks { get; set; } = new List<HabitusTask>();
        public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
        public virtual ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();
        
        public bool IsCompleted => Status == Enums.TaskStatus.Completed;
        public bool IsOverdue => DueDate.HasValue && DueDate < DateTime.UtcNow && !IsCompleted;
    }
}
