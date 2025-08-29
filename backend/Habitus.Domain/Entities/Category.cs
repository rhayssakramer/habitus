using System;
using System.Collections.Generic;

namespace Habitus.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = "#007bff"; // Cor padrão azul
        public string? Icon { get; set; }
        public Guid UserId { get; set; }
        
        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
        public virtual ICollection<Habit> Habits { get; set; } = new List<Habit>();
    }
}
