namespace MeusPila.Application.UseCases.User.CreateUser;

public record CreateUserOutputDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
