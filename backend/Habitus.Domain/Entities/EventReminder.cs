using System;

namespace Habitus.Domain.Entities
{
    public class EventReminder : BaseEntity
    {
        public DateTime ReminderTime { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsEmailReminder { get; set; } = false;
        public bool IsPushNotification { get; set; } = true;
        public bool IsSent { get; set; } = false;
        public Guid CalendarEventId { get; set; }
        
        // Navigation Properties
        public virtual CalendarEvent CalendarEvent { get; set; } = null!;
    }
}
