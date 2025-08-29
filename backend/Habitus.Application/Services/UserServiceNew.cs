using Habitus.Application.DTOs.Auth;
using Habitus.Application.DTOs.Admin;
using Habitus.Application.DTOs.User;
using Habitus.Domain.Entities;
using Habitus.Domain.Enums;
using Habitus.Domain.Interfaces;
using Habitus.Domain.Interfaces.Services;
using Habitus.Domain.Interfaces.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Habitus.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            IUserRepository userRepository, 
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        #region User Management

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _userRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                throw;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                return await _userRepository.GetByEmailAsync(email);
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                throw;
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                return await _userRepository.GetByUsernameAsync(username);
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string email, string? username = null)
        {
            try
            {
                var emailExists = await _userRepository.EmailExistsAsync(email);
                if (emailExists) return true;

                if (!string.IsNullOrEmpty(username))
                {
                    var usernameExists = await _userRepository.UsernameExistsAsync(username);
                    return usernameExists;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                throw;
            }
        }

        #endregion

        #region User Registration

        public async Task<User> CreateUserAsync(object registerDto)
        {
            try
            {
                var dto = (RegisterRequestDto)registerDto;
                
                // Verificar se já existe
                if (await ExistsAsync(dto.Email))
                {
                    throw new InvalidOperationException("Email já cadastrado");
                }

                // Criar usuário
                var user = new User
                {
                    Email = dto.Email.ToLower(),
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PasswordHash = HashPassword(dto.Password),
                    Role = UserRole.User,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                throw;
            }
        }

        #endregion

        #region Password Management

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var salt = "HabitusSalt2024"; // Em produção, usar salt único por usuário
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            return Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            return hash == hashedPassword;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (user == null) return false;

                if (!VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return false;
                }

                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            try
            {
                var user = await GetByEmailAsync(email);
                if (user == null) return false;

                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                return false;
            }
        }

        #endregion

        #region Profile Management

        public async Task<bool> UpdateProfileAsync(int userId, object profileDto)
        {
            try
            {
                var dto = (UpdateProfileRequestDto)profileDto;
                var user = await GetByIdAsync(userId);
                if (user == null) return false;

                // Verificar se email já existe (se foi alterado)
                if (user.Email != dto.Email.ToLower())
                {
                    var emailExists = await _userRepository.EmailExistsAsync(dto.Email);
                    if (emailExists)
                    {
                        throw new InvalidOperationException("Email já está em uso");
                    }
                }

                // Atualizar dados
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Email = dto.Email.ToLower();
                user.UpdatedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                throw;
            }
        }

        public async Task<bool> ConfirmEmailAsync(int userId)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (user == null) return false;

                user.UpdatedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                return false;
            }
        }

        #endregion

        #region Admin Functions

        public async Task<IEnumerable<User>> GetAllUsersAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                return await _userRepository.GetPagedAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                throw;
            }
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            try
            {
                return await _userRepository.CountAsync();
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                throw;
            }
        }

        public async Task<bool> ToggleUserStatusAsync(int userId)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (user == null) return false;

                user.IsActive = !user.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                return false;
            }
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, UserRole newRole)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (user == null) return false;

                user.Role = newRole;
                user.UpdatedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                return false;
            }
        }

        public async Task<object> GetDashboardStatsAsync()
        {
            try
            {
                var totalUsers = await GetTotalUsersCountAsync();
                var activeUsers = await _userRepository.GetActiveUsersCountAsync();
                var inactiveUsers = await _userRepository.GetInactiveUsersCountAsync();
                var adminUsers = await _userRepository.CountAsync(u => u.Role == UserRole.Admin);

                var recentUsers = await _userRepository.GetRecentAsync(5);

                return new AdminDashboardDto
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    InactiveUsers = inactiveUsers,
                    AdminUsers = adminUsers,
                    RecentUsers = recentUsers.Select(u => new UserSummaryDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Role = u.Role,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                throw;
            }
        }

        #endregion

        #region Cleanup

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (user == null) return false;

                // Soft delete
                _userRepository.Delete(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log error - implementar logger depois
                return false;
            }
        }

        #endregion
    }
}
