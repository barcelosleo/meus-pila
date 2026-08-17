namespace MeusPila.Application.UseCases.User.GetUser;

public record GetUserOutputDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string CreatedById { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required string UpdatedById { get; init; }
}
