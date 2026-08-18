namespace MeusPila.Application.UseCases.Houshold.ListHouseholds;

public record HouseholdDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record ListHouseholdsOutputDto
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }

    public IEnumerable<HouseholdDto> Items { get; init; } = [];
}
