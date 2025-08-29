using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Habitus.Application.DTOs.Admin;
using Habitus.Domain.Interfaces;
using Habitus.Domain.Enums;
using System.Security.Claims;

namespace Habitus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IUnitOfWork unitOfWork, ILogger<AdminController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Obtém dados do dashboard administrativo
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
        {
            try
            {
                var allUsers = await _unitOfWork.Users.GetAllAsync();
                var allTasks = await _unitOfWork.Tasks.GetAllAsync();
                var allHabits = await _unitOfWork.Habits.GetAllAsync();
                var allEvents = await _unitOfWork.CalendarEvents.GetAllAsync();

                var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                var newUsersThisMonth = allUsers.Count(u => u.CreatedAt >= startOfMonth);

                // Dados do gráfico de registro de usuários (últimos 30 dias)
                var userRegistrationChart = new List<UserRegistrationChartDto>();
                for (int i = 29; i >= 0; i--)
                {
                    var date = DateTime.UtcNow.Date.AddDays(-i);
                    var count = allUsers.Count(u => u.CreatedAt.Date == date);
                    userRegistrationChart.Add(new UserRegistrationChartDto
                    {
                        Date = date,
                        Count = count
                    });
                }

                // Dados do gráfico de conclusão de tarefas (últimos 7 dias)
                var taskCompletionChart = new List<TaskCompletionChartDto>();
                for (int i = 6; i >= 0; i--)
                {
                    var date = DateTime.UtcNow.Date.AddDays(-i);
                    var tasksOnDate = allTasks.Where(t => t.CreatedAt.Date <= date);
                    var completedTasksOnDate = tasksOnDate.Where(t => t.CompletedAt?.Date <= date);
                    
                    taskCompletionChart.Add(new TaskCompletionChartDto
                    {
                        Date = date,
                        TotalTasks = tasksOnDate.Count(),
                        CompletedTasks = completedTasksOnDate.Count()
                    });
                }

                // Usuários recentes
                var recentUsers = allUsers
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(10)
                    .Select(u => new AdminUserDto
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Role = u.Role,
                        IsEmailConfirmed = u.IsEmailConfirmed,
                        IsActive = u.IsActive,
                        LastLoginAt = u.LastLoginAt,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt,
                        LoginProvider = u.LoginProvider,
                        TotalTasks = allTasks.Count(t => t.UserId == u.Id),
                        CompletedTasks = allTasks.Count(t => t.UserId == u.Id && t.IsCompleted),
                        ActiveHabits = allHabits.Count(h => h.UserId == u.Id && h.IsActive),
                        TotalCalendarEvents = allEvents.Count(e => e.UserId == u.Id)
                    })
                    .ToList();

                var dashboard = new AdminDashboardDto
                {
                    TotalUsers = allUsers.Count(),
                    ActiveUsers = allUsers.Count(u => u.IsActive),
                    NewUsersThisMonth = newUsersThisMonth,
                    TotalTasks = allTasks.Count(),
                    CompletedTasks = allTasks.Count(t => t.IsCompleted),
                    TotalHabits = allHabits.Count(),
                    TotalEvents = allEvents.Count(),
                    UserRegistrationChart = userRegistrationChart,
                    TaskCompletionChart = taskCompletionChart,
                    RecentUsers = recentUsers
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do dashboard");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Lista todos os usuários com paginação
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] UserRole? role = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var allUsers = await _unitOfWork.Users.GetAllAsync();
                var allTasks = await _unitOfWork.Tasks.GetAllAsync();
                var allHabits = await _unitOfWork.Habits.GetAllAsync();
                var allEvents = await _unitOfWork.CalendarEvents.GetAllAsync();

                var query = allUsers.AsQueryable();

                // Filtros
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(u => 
                        u.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        u.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        u.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                if (role.HasValue)
                {
                    query = query.Where(u => u.Role == role.Value);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == isActive.Value);
                }

                // Paginação
                var totalUsers = query.Count();
                var users = query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new AdminUserDto
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Role = u.Role,
                        IsEmailConfirmed = u.IsEmailConfirmed,
                        IsActive = u.IsActive,
                        LastLoginAt = u.LastLoginAt,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt,
                        LoginProvider = u.LoginProvider,
                        TotalTasks = allTasks.Count(t => t.UserId == u.Id),
                        CompletedTasks = allTasks.Count(t => t.UserId == u.Id && t.IsCompleted),
                        ActiveHabits = allHabits.Count(h => h.UserId == u.Id && h.IsActive),
                        TotalCalendarEvents = allEvents.Count(e => e.UserId == u.Id)
                    })
                    .ToList();

                Response.Headers.Add("X-Total-Count", totalUsers.ToString());
                Response.Headers.Add("X-Page", page.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuários");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém detalhes de um usuário específico
        /// </summary>
        [HttpGet("users/{id}")]
        public async Task<ActionResult<AdminUserDto>> GetUser(Guid id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                var userTasks = await _unitOfWork.Tasks.GetByUserIdAsync(id);
                var userHabits = await _unitOfWork.Habits.GetByUserIdAsync(id);
                var userEvents = await _unitOfWork.CalendarEvents.GetByUserIdAsync(id);

                var adminUser = new AdminUserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    IsActive = user.IsActive,
                    LastLoginAt = user.LastLoginAt,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    LoginProvider = user.LoginProvider,
                    TotalTasks = userTasks.Count(),
                    CompletedTasks = userTasks.Count(t => t.IsCompleted),
                    ActiveHabits = userHabits.Count(h => h.IsActive),
                    TotalCalendarEvents = userEvents.Count()
                };

                return Ok(adminUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuário {UserId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualiza o role de um usuário
        /// </summary>
        [HttpPut("users/{id}/role")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleDto request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                user.Role = request.NewRole;
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new { message = "Role do usuário atualizada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar role do usuário {UserId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualiza o status (ativo/inativo) de um usuário
        /// </summary>
        [HttpPut("users/{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusDto request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                user.IsActive = request.IsActive;
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var action = request.IsActive ? "ativado" : "desativado";
                return Ok(new { message = $"Usuário {action} com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do usuário {UserId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém logs de auditoria
        /// </summary>
        [HttpGet("audit-logs")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] Guid? userId = null,
            [FromQuery] string? entityName = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var allLogs = await _unitOfWork.AuditLogs.GetAllAsync();
                var query = allLogs.AsQueryable();

                // Filtros
                if (userId.HasValue)
                {
                    query = query.Where(l => l.UserId == userId.Value);
                }

                if (!string.IsNullOrEmpty(entityName))
                {
                    query = query.Where(l => l.EntityName.Equals(entityName, StringComparison.OrdinalIgnoreCase));
                }

                if (startDate.HasValue)
                {
                    query = query.Where(l => l.CreatedAt >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(l => l.CreatedAt <= endDate.Value);
                }

                // Paginação
                var totalLogs = query.Count();
                var logs = query
                    .OrderByDescending(l => l.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new AuditLogDto
                    {
                        Id = l.Id,
                        Action = l.Action,
                        EntityName = l.EntityName,
                        EntityId = l.EntityId,
                        OldValues = l.OldValues,
                        NewValues = l.NewValues,
                        IpAddress = l.IpAddress,
                        UserAgent = l.UserAgent,
                        CreatedAt = l.CreatedAt,
                        UserName = l.User != null ? l.User.FullName : null,
                        UserEmail = l.User?.Email
                    })
                    .ToList();

                Response.Headers.Add("X-Total-Count", totalLogs.ToString());
                Response.Headers.Add("X-Page", page.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter logs de auditoria");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Deleta um usuário (soft delete)
        /// </summary>
        [HttpDelete("users/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (id == currentUserId)
                {
                    return BadRequest(new { message = "Não é possível deletar sua própria conta" });
                }

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                await _unitOfWork.Users.DeleteAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new { message = "Usuário deletado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuário {UserId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        #region Private Methods

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;
            
            throw new UnauthorizedAccessException("Usuário não autenticado");
        }

        #endregion
    }
}
