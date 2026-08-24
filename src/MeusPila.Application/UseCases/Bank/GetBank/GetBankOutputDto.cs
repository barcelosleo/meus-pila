namespace MeusPila.Application.UseCases.Bank.GetBank;

public record GetBankOutputDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required DateTime CreatedAt { get; init; }
    public string? CreatedById { get; init; }
    public string? CreatedByName { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedById { get; init; }
    public string? UpdatedByName { get; init; }
}
