namespace MeusPila.Application.UseCases.User.GetUser;

public record GetUserOutputDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Role { get; init; }
    public required DateTime CreatedAt { get; init; }
    public string? CreatedById { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedById { get; init; }
}
