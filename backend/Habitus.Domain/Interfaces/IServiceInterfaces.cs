using System;
using System.Collections.Generic;
using Habitus.Domain.Entities;
using Habitus.Domain.Enums;

namespace Habitus.Domain.Interfaces.Services
{
    public interface IUserService
    {
        // User Management
        System.Threading.Tasks.Task<User?> GetByIdAsync(int id);
        System.Threading.Tasks.Task<User?> GetByEmailAsync(string email);
        System.Threading.Tasks.Task<User?> GetByUsernameAsync(string username);
        System.Threading.Tasks.Task<bool> ExistsAsync(string email, string? username = null);

        // User Registration
        System.Threading.Tasks.Task<User> CreateUserAsync(object registerDto);

        // Password Management
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        System.Threading.Tasks.Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        System.Threading.Tasks.Task<bool> ResetPasswordAsync(string email, string newPassword);

        // Profile Management
        System.Threading.Tasks.Task<bool> UpdateProfileAsync(int userId, object profileDto);
        System.Threading.Tasks.Task<bool> ConfirmEmailAsync(int userId);

        // Admin Functions
        System.Threading.Tasks.Task<IEnumerable<User>> GetAllUsersAsync(int page = 1, int pageSize = 10);
        System.Threading.Tasks.Task<int> GetTotalUsersCountAsync();
        System.Threading.Tasks.Task<bool> ToggleUserStatusAsync(int userId);
        System.Threading.Tasks.Task<bool> UpdateUserRoleAsync(int userId, UserRole newRole);
        System.Threading.Tasks.Task<object> GetDashboardStatsAsync();

        // Cleanup
        System.Threading.Tasks.Task<bool> DeleteUserAsync(int userId);
    }

    public interface IAuthService
    {
        System.Threading.Tasks.Task<string> GenerateJwtTokenAsync(User user);
        System.Threading.Tasks.Task<RefreshToken> GenerateRefreshTokenAsync(string ipAddress);
        System.Threading.Tasks.Task<bool> ValidateTokenAsync(string token);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        System.Threading.Tasks.Task<object> LoginAsync(string email, string password);
        System.Threading.Tasks.Task<object> RegisterAsync(string email, string password, string firstName, string lastName, string? username = null);
        System.Threading.Tasks.Task<object> RefreshTokenAsync(string refreshToken);
        System.Threading.Tasks.Task<bool> RevokeTokenAsync(string refreshToken);
        System.Threading.Tasks.Task<bool> ForgotPasswordAsync(string email);
        System.Threading.Tasks.Task<bool> ResetPasswordAsync(string token, string newPassword);
    }

    public interface IEmailService
    {
        System.Threading.Tasks.Task<bool> SendEmailAsync(string to, string subject, string body);
        System.Threading.Tasks.Task<bool> SendWelcomeEmailAsync(string to, string firstName);
        System.Threading.Tasks.Task<bool> SendPasswordResetEmailAsync(string to, string resetToken);
        System.Threading.Tasks.Task<bool> SendEmailConfirmationAsync(string to, string confirmationToken);
    }
}
