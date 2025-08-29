using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Habitus.Domain.Entities;
using Habitus.Domain.Enums;

namespace Habitus.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokenAsync(User user);
        Task<RefreshToken> GenerateRefreshTokenAsync(string ipAddress);
        Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }

    public interface IEmailService
    {
        System.Threading.Tasks.Task SendEmailConfirmationAsync(string email, string token);
        System.Threading.Tasks.Task SendPasswordResetAsync(string email, string token);
        System.Threading.Tasks.Task SendWelcomeEmailAsync(string email, string firstName);
    }

    public interface IUserService
    {
        // User Management
        System.Threading.Tasks.Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> ExistsAsync(string email, string? username = null);

        // User Registration
        Task<User> CreateUserAsync(object registerDto);

        // Password Management
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email, string newPassword);

        // Profile Management
        Task<bool> UpdateProfileAsync(int userId, object profileDto);
        Task<bool> ConfirmEmailAsync(int userId);

        // Admin Functions
        Task<IEnumerable<User>> GetAllUsersAsync(int page = 1, int pageSize = 10);
        Task<int> GetTotalUsersCountAsync();
        Task<bool> ToggleUserStatusAsync(int userId);
        Task<bool> UpdateUserRoleAsync(int userId, UserRole newRole);
        Task<object> GetDashboardStatsAsync();

        // Cleanup
        Task<bool> DeleteUserAsync(int userId);
    }
}
