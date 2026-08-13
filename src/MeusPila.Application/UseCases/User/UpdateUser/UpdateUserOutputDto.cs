namespace MeusPila.Application.UseCases.User.UpdateUser;

public record UpdateUserOutputDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
