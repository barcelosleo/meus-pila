using MeusPila.Domain.Common;

namespace MeusPila.Application.UseCases.Group.ListGroups;

public record ListGroupsInputDto
{
    public required string RequesterUserId { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = PagedListProperties.DefaultPageSize;
}

public record ListGroupsQueryDto
{
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = PagedListProperties.DefaultPageSize;
}
