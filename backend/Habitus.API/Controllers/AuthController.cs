using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Habitus.Application.DTOs.Auth;
using Habitus.Domain.Interfaces;
using System.Security.Claims;

namespace Habitus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var user = await _userService.RegisterAsync(
                    request.FirstName, 
                    request.LastName, 
                    request.Email, 
                    request.Password);

                var response = new AuthResponseDto
                {
                    Success = true,
                    Message = "Usuário registrado com sucesso. Verifique seu email para confirmar a conta.",
                    User = new UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Role = user.Role,
                        IsEmailConfirmed = user.IsEmailConfirmed,
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt
                    }
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Erro interno do servidor",
                    Errors = new List<string> { "Erro interno do servidor" }
                });
            }
        }

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var ipAddress = GetIpAddress();
                var (user, token, refreshToken) = await _userService.LoginAsync(
                    request.Email, 
                    request.Password, 
                    ipAddress);

                var response = new AuthResponseDto
                {
                    Success = true,
                    Message = "Login realizado com sucesso",
                    AccessToken = token,
                    RefreshToken = refreshToken.Token,
                    ExpiresAt = DateTime.UtcNow.AddHours(1), // JWT válido por 1 hora
                    User = new UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Role = user.Role,
                        IsEmailConfirmed = user.IsEmailConfirmed,
                        IsActive = user.IsActive,
                        LastLoginAt = user.LastLoginAt,
                        CreatedAt = user.CreatedAt
                    }
                };

                // Configurar cookie seguro para refresh token
                SetRefreshTokenCookie(refreshToken.Token);

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar login");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Erro interno do servidor",
                    Errors = new List<string> { "Erro interno do servidor" }
                });
            }
        }

        /// <summary>
        /// Atualiza o token de acesso usando refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto? request = null)
        {
            try
            {
                var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];
                
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new { message = "Refresh token é obrigatório" });
                }

                var ipAddress = GetIpAddress();
                var (newToken, newRefreshToken) = await _userService.RefreshTokenAsync(refreshToken, ipAddress);

                SetRefreshTokenCookie(newRefreshToken.Token);

                return Ok(new RefreshTokenResponseDto
                {
                    AccessToken = newToken,
                    RefreshToken = newRefreshToken.Token,
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar token");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Revoga o refresh token (logout)
        /// </summary>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestDto? request = null)
        {
            try
            {
                var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];
                
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new { message = "Refresh token é obrigatório" });
                }

                var ipAddress = GetIpAddress();
                await _userService.RevokeTokenAsync(refreshToken, ipAddress);
                
                // Remove cookie
                Response.Cookies.Delete("refreshToken");

                return Ok(new { message = "Token revogado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao revogar token");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Confirma email do usuário
        /// </summary>
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            try
            {
                var result = await _userService.ConfirmEmailAsync(token);
                
                if (result)
                {
                    return Ok(new { message = "Email confirmado com sucesso" });
                }
                
                return BadRequest(new { message = "Token inválido ou expirado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao confirmar email");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Solicita redefinição de senha
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                await _userService.ForgotPasswordAsync(request.Email);
                return Ok(new { message = "Se o email existir, um link de redefinição será enviado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao solicitar redefinição de senha");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Redefine a senha do usuário
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            try
            {
                var result = await _userService.ResetPasswordAsync(request.Token, request.NewPassword);
                
                if (result)
                {
                    return Ok(new { message = "Senha redefinida com sucesso" });
                }
                
                return BadRequest(new { message = "Token inválido ou expirado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao redefinir senha");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Altera a senha do usuário logado
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
                
                if (result)
                {
                    return Ok(new { message = "Senha alterada com sucesso" });
                }
                
                return BadRequest(new { message = "Senha atual incorreta" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém informações do usuário logado
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userService.GetUserByIdAsync(userId);
                
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                return Ok(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ProfilePicture = user.ProfilePicture,
                    Role = user.Role,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    IsActive = user.IsActive,
                    LastLoginAt = user.LastLoginAt,
                    CreatedAt = user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuário atual");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        #region Private Methods

        private void SetRefreshTokenCookie(string token)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            
            Response.Cookies.Append("refreshToken", token, options);
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
            
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

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
