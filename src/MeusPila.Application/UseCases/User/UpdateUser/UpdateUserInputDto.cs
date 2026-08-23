using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.User.UpdateUser;

public record UpdateUserQueryDto
{
    [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
    public required string Id { get; init; }

    [StringLength(100, ErrorMessage = "O nome do usuário não pode exceder 100 caracteres.")]
    public string? Name { get; init; }

    [EmailAddress(ErrorMessage = "O email informado não é válido.")]
    public string? Email { get; init; }

    public string? CurrentPassword { get; init; }

    [StringLength(64, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 64 caracteres.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,64}$",
        ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial."
    )]
    public string? NewPassword { get; init; }
    public bool? IsActive { get; init; }
}

public record UpdateUserInputDto
{
    public required string Id { get; init; }
    public string? Name { get; init; }
    public string? Email { get; init; }

    public string? CurrentPassword { get; init; }

    public string? NewPassword { get; init; }
    public bool? IsActive { get; init; }

    public required string UpdatedById { get; init; }
}
