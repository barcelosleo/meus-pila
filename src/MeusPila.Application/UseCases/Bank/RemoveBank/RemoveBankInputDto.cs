namespace MeusPila.Application.UseCases.Bank.RemoveBank;

public record RemoveBankInputDto
{
    public required string Id { get; init; }

    public required string RequesterId { get; init; }
}
