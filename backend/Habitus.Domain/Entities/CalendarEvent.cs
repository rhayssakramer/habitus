using System;
using System.Collections.Generic;

namespace Habitus.Domain.Entities
{
    public class CalendarEvent : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAllDay { get; set; } = false;
        public string Color { get; set; } = "#007bff";
        public string? Location { get; set; }
        public bool IsRecurring { get; set; } = false;
        public string? RecurrenceRule { get; set; } // RRULE format
        public Guid UserId { get; set; }
        public Guid? TaskId { get; set; } // Link opcional com Task
        
        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Task? Task { get; set; }
        public virtual ICollection<EventReminder> Reminders { get; set; } = new List<EventReminder>();
    }
}
