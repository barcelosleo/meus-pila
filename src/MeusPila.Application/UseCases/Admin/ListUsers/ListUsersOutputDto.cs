using MeusPila.Domain.Interfaces;

namespace MeusPila.Application.UseCases.Admin.ListUsers;

public record UserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? CreatedById { get; init; }
    public string? CreatedByName { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedById { get; init; }
    public string? UpdatedByName { get; init; }
}

public record ListUsersOutputDto : IPagedList<UserDto>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public IEnumerable<UserDto> Items { get; init; } = [];
}
