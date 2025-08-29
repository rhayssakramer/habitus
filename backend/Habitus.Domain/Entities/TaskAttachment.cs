using System;

namespace Habitus.Domain.Entities
{
    public class TaskAttachment : BaseEntity
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        
        // Navigation Properties
        public virtual Task Task { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
