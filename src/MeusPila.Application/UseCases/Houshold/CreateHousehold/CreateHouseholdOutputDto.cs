namespace MeusPila.Application.UseCases.Houshold.CreateHousehold;

public record CreateHouseholdOutputDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string OwnerId { get; init; }
    public required string CreatedById { get; init; }
    public required DateTime CreatedAt { get; init; }
}
