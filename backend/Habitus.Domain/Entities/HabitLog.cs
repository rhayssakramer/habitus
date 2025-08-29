using System;

namespace Habitus.Domain.Entities
{
    public class HabitLog : BaseEntity
    {
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public string? Notes { get; set; }
        public Guid HabitId { get; set; }
        public Guid UserId { get; set; }
        
        // Navigation Properties
        public virtual Habit Habit { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
