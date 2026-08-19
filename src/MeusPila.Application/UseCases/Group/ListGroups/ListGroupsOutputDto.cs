namespace MeusPila.Application.UseCases.Group.ListGroups;

public record GroupDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? CreatedById { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedById { get; init; }
}

public record ListGroupsOutputDto
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }

    public IEnumerable<GroupDto> Items { get; init; } = [];
}
