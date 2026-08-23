using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.Admin.CreateUser;

public record CreateUserInputDto
{
    [Required(ErrorMessage = "O nome do usuário é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do usuário não pode exceder 100 caracteres.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "O email do usuário é obrigatório.")]
    [EmailAddress(ErrorMessage = "O email informado não é válido.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "A senha do usuário é obrigatória.")]
    [StringLength(64, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 64 caracteres.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,64}$",
        ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial."
    )]
    public string Password { get; init; } = string.Empty;

    [Required(ErrorMessage = "O papel do usuário é obrigatório.")]
    [RegularExpression(
        "Admin|User|GroupManager",
        ErrorMessage = "O papel do usuário deve ser 'Admin', 'User' ou 'GroupManager'."
    )]
    public string Role { get; init; } = string.Empty;

    public string? CreatedById { get; init; }
}
