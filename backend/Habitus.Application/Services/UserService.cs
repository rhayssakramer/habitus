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

                // Log success - implementar logger depois
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

                // Log success - implementar logger depois
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

                // Log success - implementar logger depois
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

                // Log success - implementar logger depois
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

                // Implementar confirmação de email quando adicionar a propriedade
                user.UpdatedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                // Log success - implementar logger depois
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

                // Log success - implementar logger depois
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

                // Log success - implementar logger depois
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

                // Log success - implementar logger depois
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

        #region User Management

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _userRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por ID: {UserId}", id);
                throw;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                var users = await _userRepository.FindAsync(u => u.Email == email.ToLower());
                return users.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por email: {Email}", email);
                throw;
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                var users = await _userRepository.FindAsync(u => u.Username == username.ToLower());
                return users.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por username: {Username}", username);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string email, string? username = null)
        {
            try
            {
                var emailExists = await _userRepository.AnyAsync(u => u.Email == email.ToLower());
                if (emailExists) return true;

                if (!string.IsNullOrEmpty(username))
                {
                    var usernameExists = await _userRepository.AnyAsync(u => u.Username == username.ToLower());
                    return usernameExists;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do usuário: {Email}, {Username}", email, username);
                throw;
            }
        }

        #endregion

        #region User Registration

        public async Task<User> CreateUserAsync(RegisterRequestDto registerDto)
        {
            try
            {
                // Verificar se já existe
                if (await ExistsAsync(registerDto.Email, registerDto.Username))
                {
                    throw new InvalidOperationException("Email ou username já cadastrados");
                }

                // Criar usuário
                var user = new User
                {
                    Username = registerDto.Username.ToLower(),
                    Email = registerDto.Email.ToLower(),
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    PasswordHash = HashPassword(registerDto.Password),
                    Role = UserRole.User,
                    IsActive = true,
                    EmailConfirmed = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Usuário criado com sucesso: {Email}", user.Email);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário: {Email}", registerDto.Email);
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

                _logger.LogInformation("Senha alterada para usuário: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha do usuário: {UserId}", userId);
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

                _logger.LogInformation("Senha resetada para usuário: {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao resetar senha do usuário: {Email}", email);
                return false;
            }
        }

        #endregion

        #region Profile Management

        public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequestDto profileDto)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (user == null) return false;

                // Verificar se email já existe (se foi alterado)
                if (user.Email != profileDto.Email.ToLower())
                {
                    var emailExists = await _userRepository.AnyAsync(u => u.Email == profileDto.Email.ToLower() && u.Id != userId);
                    if (emailExists)
                    {
                        throw new InvalidOperationException("Email já está em uso");
                    }
                }

                // Atualizar dados
                user.FirstName = profileDto.FirstName;
                user.LastName = profileDto.LastName;
                user.Email = profileDto.Email.ToLower();
                user.UpdatedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Perfil atualizado para usuário: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar perfil do usuário: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ConfirmEmailAsync(int userId)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (user == null) return false;

                user.EmailConfirmed = true;
                user.UpdatedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Email confirmado para usuário: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao confirmar email do usuário: {UserId}", userId);
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
                _logger.LogError(ex, "Erro ao buscar todos os usuários");
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
                _logger.LogError(ex, "Erro ao contar total de usuários");
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

                _logger.LogInformation("Status do usuário alterado: {UserId} - Ativo: {IsActive}", userId, user.IsActive);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do usuário: {UserId}", userId);
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

                _logger.LogInformation("Role do usuário alterado: {UserId} - Nova Role: {Role}", userId, newRole);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar role do usuário: {UserId}", userId);
                return false;
            }
        }

        public async Task<AdminDashboardDto> GetDashboardStatsAsync()
        {
            try
            {
                var totalUsers = await GetTotalUsersCountAsync();
                var activeUsers = await _userRepository.CountAsync(u => u.IsActive);
                var inactiveUsers = totalUsers - activeUsers;
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
                        Username = u.Username,
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
                _logger.LogError(ex, "Erro ao buscar estatísticas do dashboard");
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
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Usuário deletado (soft): {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuário: {UserId}", userId);
                return false;
            }
        }

        #endregion
    }
}
