using System;
using System.Collections.Generic;
using Habitus.Domain.Enums;

namespace Habitus.Domain.Entities
{
    public class User : BaseEntity
    {
        // Dados básicos
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        
        // Dados pessoais
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePicture { get; set; }
        
        // Endereço completo
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? Complement { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; } = "Brasil";
        
        // Configurações de conta
        public DateTime? LastLoginAt { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsActive { get; set; } = true;
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailConfirmationTokenExpiry { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public LoginProvider LoginProvider { get; set; } = LoginProvider.Local;
        public string? ExternalUserId { get; set; }
        
        // Navigation Properties
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<Habit> Habits { get; set; } = new List<Habit>();
        public virtual ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();
        
        // Computed Properties
        public string FullName => $"{FirstName} {LastName}";
        public string FullAddress => string.Join(", ", new[] 
        { 
            !string.IsNullOrEmpty(Street) && !string.IsNullOrEmpty(Number) ? $"{Street}, {Number}" : null,
            !string.IsNullOrEmpty(Complement) ? Complement : null,
            !string.IsNullOrEmpty(Neighborhood) ? Neighborhood : null,
            !string.IsNullOrEmpty(City) ? City : null,
            !string.IsNullOrEmpty(State) ? State : null,
            !string.IsNullOrEmpty(ZipCode) ? ZipCode : null
        }.Where(x => !string.IsNullOrEmpty(x)));
        
        public int? Age 
        { 
            get 
            { 
                if (!DateOfBirth.HasValue) return null;
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                return age;
            } 
        }
        
        public bool IsProfileComplete => 
            !string.IsNullOrEmpty(FirstName) &&
            !string.IsNullOrEmpty(LastName) &&
            !string.IsNullOrEmpty(Email) &&
            !string.IsNullOrEmpty(Phone) &&
            DateOfBirth.HasValue &&
            !string.IsNullOrEmpty(Street) &&
            !string.IsNullOrEmpty(Number) &&
            !string.IsNullOrEmpty(City) &&
            !string.IsNullOrEmpty(State) &&
            !string.IsNullOrEmpty(ZipCode);
    }
}
