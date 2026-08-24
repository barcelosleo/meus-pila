namespace MeusPila.Application.UseCases.Bank.CreateBank;

public record CreateBankOutputDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required DateTime CreatedAt { get; init; }
}
