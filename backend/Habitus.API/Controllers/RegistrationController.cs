using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Habitus.Application.DTOs.Registration;
using Habitus.Application.DTOs.Auth;
using Habitus.Domain.Interfaces.Services;
using Habitus.Domain.Entities;
using System.Security.Claims;

namespace Habitus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public RegistrationController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        /// <summary>
        /// Registra um novo usuário com dados completos
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterComplete([FromBody] RegisterCompleteRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        message = "Dados inválidos", 
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                    });
                }

                // Verificar se o usuário já existe
                if (await _userService.ExistsAsync(request.Email))
                {
                    return BadRequest(new { message = "Email já cadastrado" });
                }

                // Criar usuário
                var user = await _userService.CreateUserAsync(request);

                // Gerar tokens de autenticação
                var jwtToken = await _authService.GenerateJwtTokenAsync(user);
                var refreshToken = await _authService.GenerateRefreshTokenAsync("127.0.0.1"); // IP address

                var response = new RegisterResponseDto
                {
                    Message = "Usuário registrado com sucesso",
                    User = new UserSummaryDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role,
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt
                    },
                    Token = jwtToken,
                    RefreshToken = refreshToken.Token,
                    TokenExpiry = DateTime.UtcNow.AddHours(1) // Configurável
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza o perfil completo do usuário logado
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateFullProfileRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        message = "Dados inválidos", 
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                    });
                }

                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Unauthorized(new { message = "Usuário não autenticado" });
                }

                var success = await _userService.UpdateProfileAsync(userId, request);
                if (!success)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                return Ok(new { message = "Perfil atualizado com sucesso" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o perfil completo do usuário logado
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Unauthorized(new { message = "Usuário não autenticado" });
                }

                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                var profile = new UserProfileCompleteDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    DateOfBirth = user.DateOfBirth,
                    ProfilePicture = user.ProfilePicture,
                    Street = user.Street,
                    Number = user.Number,
                    Complement = user.Complement,
                    Neighborhood = user.Neighborhood,
                    City = user.City,
                    State = user.State,
                    ZipCode = user.ZipCode,
                    Country = user.Country,
                    FullName = user.FullName,
                    FullAddress = user.FullAddress,
                    Age = user.Age,
                    IsProfileComplete = user.IsProfileComplete,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Upload de foto de perfil
        /// </summary>
        [HttpPost("profile/picture")]
        [Authorize]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Nenhum arquivo foi enviado" });
                }

                // Validar tipo de arquivo
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(new { message = "Tipo de arquivo não permitido. Use JPG, PNG ou GIF." });
                }

                // Validar tamanho (máximo 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Arquivo muito grande. Máximo 5MB." });
                }

                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Unauthorized(new { message = "Usuário não autenticado" });
                }

                // Converter para bytes
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                
                var uploadDto = new UploadProfilePictureDto
                {
                    ImageData = memoryStream.ToArray(),
                    FileName = file.FileName,
                    ContentType = file.ContentType
                };

                // TODO: Implementar service para upload de imagem
                // var imagePath = await _imageService.SaveProfilePictureAsync(userId, uploadDto);

                // Por enquanto, retornar sucesso simulado
                var imagePath = $"/uploads/profiles/{userId}_{file.FileName}";

                return Ok(new { 
                    message = "Foto de perfil enviada com sucesso",
                    imagePath = imagePath
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Remove a foto de perfil
        /// </summary>
        [HttpDelete("profile/picture")]
        [Authorize]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Unauthorized(new { message = "Usuário não autenticado" });
                }

                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                // TODO: Implementar remoção da imagem
                // await _imageService.RemoveProfilePictureAsync(userId);

                return Ok(new { message = "Foto de perfil removida com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Verificar se email está disponível
        /// </summary>
        [HttpGet("check-email/{email}")]
        public async Task<IActionResult> CheckEmailAvailability(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Email é obrigatório" });
                }

                var exists = await _userService.ExistsAsync(email);
                
                return Ok(new { 
                    available = !exists,
                    message = exists ? "Email já está em uso" : "Email disponível"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Buscar CEP via API externa
        /// </summary>
        [HttpGet("cep/{cep}")]
        public async Task<IActionResult> GetAddressByCep(string cep)
        {
            try
            {
                if (string.IsNullOrEmpty(cep))
                {
                    return BadRequest(new { message = "CEP é obrigatório" });
                }

                // Limpar CEP
                cep = cep.Replace("-", "").Replace(".", "");

                if (cep.Length != 8)
                {
                    return BadRequest(new { message = "CEP deve ter 8 dígitos" });
                }

                // TODO: Implementar integração com API de CEP (ViaCEP)
                // var address = await _cepService.GetAddressByCepAsync(cep);

                // Retornar dados simulados por enquanto
                var address = new
                {
                    cep = cep,
                    street = "Rua Exemplo",
                    neighborhood = "Centro",
                    city = "São Paulo",
                    state = "SP"
                };

                return Ok(address);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        #region Helper Methods

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        #endregion
    }
}
