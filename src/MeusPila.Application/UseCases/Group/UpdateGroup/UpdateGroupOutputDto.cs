namespace MeusPila.Application.UseCases.Group.UpdateGroup;

public record UpdateGroupOutputDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string OwnerId { get; init; }
    public required string UpdatedById { get; init; }
    public required DateTime UpdateAt { get; init; }
}
