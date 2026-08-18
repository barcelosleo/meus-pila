using MeusPila.Domain.Common;

namespace MeusPila.Application.UseCases.Houshold.ListHouseholds;

public record ListHouseholdsInputDto
{
    public required string RequesterUserId { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = PagedListProperties.DefaultPageSize;
}

public record ListHouseholdsQueryDto
{
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = PagedListProperties.DefaultPageSize;
}
