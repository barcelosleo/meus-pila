namespace MeusPila.Application.UseCases.User.AuthenticateUser;

using System.ComponentModel.DataAnnotations;

public record AuthenticateUserInputDto
{
    [Required(ErrorMessage = "O email do usuário é obrigatório.")]
    [EmailAddress(ErrorMessage = "O email informado não é válido.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "A senha do usuário é obrigatória.")]
    public string Password { get; init; } = string.Empty;
}
