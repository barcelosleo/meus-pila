using MeusPila.Domain.Common;

namespace MeusPila.Application.UseCases.User.ListUsers;

public record ListUsersInputDto
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = PagedListProperties.DefaultPageSize;
    public string? SearchTerm { get; init; }
}
