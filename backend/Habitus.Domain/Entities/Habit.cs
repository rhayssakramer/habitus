using System;
using System.Collections.Generic;
using Habitus.Domain.Enums;

namespace Habitus.Domain.Entities
{
    public class Habit : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = "#28a745"; // Verde padrão
        public string? Icon { get; set; }
        public int TargetFrequency { get; set; } = 1; // Quantas vezes por período
        public RecurrenceType FrequencyPeriod { get; set; } = RecurrenceType.Daily;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int CurrentStreak { get; set; } = 0;
        public int BestStreak { get; set; } = 0;
        public Guid UserId { get; set; }
        public Guid? CategoryId { get; set; }
        
        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Category? Category { get; set; }
        public virtual ICollection<HabitLog> HabitLogs { get; set; } = new List<HabitLog>();
    }
}
