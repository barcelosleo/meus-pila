namespace MeusPila.Application.UseCases.User.RefreshToken;

using System.ComponentModel.DataAnnotations;

public record RefreshTokenInputDto
{
    [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
    public Guid UserId { get; init; }

    [Required(ErrorMessage = "O refresh token é obrigatório.")]
    public string RefreshToken { get; init; } = string.Empty;
}
