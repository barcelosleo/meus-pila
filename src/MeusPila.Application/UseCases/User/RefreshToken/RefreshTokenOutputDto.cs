namespace MeusPila.Application.UseCases.User.RefreshToken;

public record RefreshTokenOutputDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}
