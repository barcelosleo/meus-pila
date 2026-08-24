namespace MeusPila.Application.UseCases.Bank.UpdateBank;

public record UpdateBankOutputDto
{
    public required string Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? CreatedById { get; init; }
    public string? CreatedByName { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedById { get; init; }
    public string? UpdatedByName { get; init; }
}
