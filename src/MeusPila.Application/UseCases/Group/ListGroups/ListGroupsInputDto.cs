using MeusPila.Domain.Common;

namespace MeusPila.Application.UseCases.Group.ListGroups;

public enum GroupsOrdering
{
    IdAsc,
    IdDesc,
    NameAsc,
    NameDesc,
    CreatedAtAsc,
    CreatedAtDesc,
    UpdatedAtAsc,
    UpdatedAtDesc,
    CreatedByAsc,
    CreatedByDesc,
}

public record ListGroupsInputDto
{
    public required string RequesterUserId { get; init; }
    public string? SearchTerm { get; init; }
    public IEnumerable<GroupsOrdering>? Ordering { get; init; } = [GroupsOrdering.CreatedAtDesc];
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = PagedListProperties.DefaultPageSize;
}

public record ListGroupsQueryDto
{
    public string? SearchTerm { get; init; }
    public IEnumerable<GroupsOrdering>? Ordering { get; init; } = [GroupsOrdering.CreatedAtDesc];
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = PagedListProperties.DefaultPageSize;
}
