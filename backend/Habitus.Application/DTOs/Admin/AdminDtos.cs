using System;
using Habitus.Domain.Enums;

namespace Habitus.Application.DTOs.Admin
{
    public class AdminUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public UserRole Role { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public LoginProvider LoginProvider { get; set; }
        
        // Stats
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int ActiveHabits { get; set; }
        public int TotalCalendarEvents { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
    }

    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int TotalHabits { get; set; }
        public int TotalEvents { get; set; }
        
        public List<UserRegistrationChartDto> UserRegistrationChart { get; set; } = new();
        public List<TaskCompletionChartDto> TaskCompletionChart { get; set; } = new();
        public List<AdminUserDto> RecentUsers { get; set; } = new();
    }

    public class UserRegistrationChartDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class TaskCompletionChartDto
    {
        public DateTime Date { get; set; }
        public int CompletedTasks { get; set; }
        public int TotalTasks { get; set; }
    }

    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
    }

    public class UpdateUserRoleDto
    {
        public Guid UserId { get; set; }
        public UserRole NewRole { get; set; }
    }

    public class UpdateUserStatusDto
    {
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
