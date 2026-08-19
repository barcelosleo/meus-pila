namespace MeusPila.Application.UseCases.User.RefreshToken;

using System.ComponentModel.DataAnnotations;

public record RefreshTokenInputDto
{
    [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
    public required Guid UserId { get; init; }

    [Required(ErrorMessage = "O refresh token é obrigatório.")]
    public required string RefreshToken { get; init; }
}
