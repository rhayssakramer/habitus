using System.ComponentModel.DataAnnotations;

namespace Habitus.Application.DTOs.Registration
{
    public class RegisterCompleteRequestDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [StringLength(50, ErrorMessage = "O sobrenome deve ter no máximo 50 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confirmação da senha é obrigatória")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Dados pessoais
        [Phone(ErrorMessage = "Formato de telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // Endereço
        [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
        public string? Street { get; set; }

        [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
        public string? Number { get; set; }

        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string? Complement { get; set; }

        [StringLength(100, ErrorMessage = "O bairro deve ter no máximo 100 caracteres")]
        public string? Neighborhood { get; set; }

        [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "O estado deve ter no máximo 50 caracteres")]
        public string? State { get; set; }

        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve estar no formato 00000-000")]
        public string? ZipCode { get; set; }

        [StringLength(50, ErrorMessage = "O país deve ter no máximo 50 caracteres")]
        public string Country { get; set; } = "Brasil";

        // Termos e condições
        [Range(typeof(bool), "true", "true", ErrorMessage = "Você deve aceitar os termos de uso")]
        public bool AcceptTerms { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Você deve aceitar a política de privacidade")]
        public bool AcceptPrivacyPolicy { get; set; }

        // Opcionais
        public bool NewsletterOptIn { get; set; } = false;
    }

    public class UpdateFullProfileRequestDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [StringLength(50, ErrorMessage = "O sobrenome deve ter no máximo 50 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Formato de telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // Endereço
        [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
        public string? Street { get; set; }

        [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
        public string? Number { get; set; }

        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string? Complement { get; set; }

        [StringLength(100, ErrorMessage = "O bairro deve ter no máximo 100 caracteres")]
        public string? Neighborhood { get; set; }

        [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "O estado deve ter no máximo 50 caracteres")]
        public string? State { get; set; }

        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve estar no formato 00000-000")]
        public string? ZipCode { get; set; }

        [StringLength(50, ErrorMessage = "O país deve ter no máximo 50 caracteres")]
        public string Country { get; set; } = "Brasil";
    }

    public class UserProfileCompleteDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePicture { get; set; }
        
        // Endereço
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? Complement { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        
        // Computed
        public string FullName { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public int? Age { get; set; }
        public bool IsProfileComplete { get; set; }
        
        // Configurações
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UploadProfilePictureDto
    {
        [Required(ErrorMessage = "A imagem é obrigatória")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        
        [Required(ErrorMessage = "O nome do arquivo é obrigatório")]
        public string FileName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O tipo do arquivo é obrigatório")]
        public string ContentType { get; set; } = string.Empty;
    }
}
