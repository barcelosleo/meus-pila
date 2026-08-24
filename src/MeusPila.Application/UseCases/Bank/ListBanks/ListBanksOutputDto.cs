using MeusPila.Domain.Interfaces;

namespace MeusPila.Application.UseCases.Bank.ListBanks;

public record BankDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? CreatedById { get; init; }
    public string? CreatedByName { get; init; } = string.Empty;
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedById { get; init; }
    public string? UpdatedByName { get; init; } = string.Empty;
}

public record ListBanksOutputDto : IPagedList<BankDto>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public IEnumerable<BankDto> Items { get; init; } = [];
}
