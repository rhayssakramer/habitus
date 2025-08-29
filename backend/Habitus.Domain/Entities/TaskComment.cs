using System;

namespace Habitus.Domain.Entities
{
    public class TaskComment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        
        // Navigation Properties
        public virtual Task Task { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
