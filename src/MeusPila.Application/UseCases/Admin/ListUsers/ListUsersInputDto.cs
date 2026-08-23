using System.Text.Json.Serialization;
using MeusPila.Domain.Common;

namespace MeusPila.Application.UseCases.Admin.ListUsers;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UsersOrdering
{
    IdAsc,
    IdDesc,
    NameAsc,
    NameDesc,
    EmailAsc,
    EmailDesc,
    RoleAsc,
    RoleDesc,
    CreatedAtAsc,
    CreatedAtDesc,
    UpdatedAtAsc,
    UpdatedAtDesc,
    CreatedByAsc,
    CreatedByDesc,
}

public record ListUsersInputDto
{
    public string? SearchTerm { get; init; }
    public IEnumerable<UsersOrdering>? Ordering { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = PagedListProperties.DefaultPageSize;
}
