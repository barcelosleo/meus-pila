using MeusPila.Application.Interfaces;
using BankEntity = MeusPila.Domain.Entities.Bank;

namespace MeusPila.Application.UseCases.Bank.GetBank;

public interface IGetBankUseCase : IUseCase<GetBankInputDto, GetBankOutputDto> { }

public class GetBankUseCase : IGetBankUseCase
{
    private readonly IBankRepository _bankRepository;

    public GetBankUseCase(IBankRepository bankRepository)
    {
        _bankRepository = bankRepository;
    }

    public async Task<GetBankOutputDto> ExecuteAsync(GetBankInputDto input)
    {
        Guid bankId = Guid.Parse(input.Id);
        BankEntity? bank = await _bankRepository.GetByIdAsync(bankId);

        if (bank is null)
        {
            throw new ArgumentException($"Banco com ID '{input.Id}' não encontrado.");
        }

        return new GetBankOutputDto
        {
            Id = bank.Id.ToString(),
            Name = bank.Name,
            Code = bank.Code,
            CreatedAt = bank.CreatedAt,
            CreatedById = bank.CreatedById?.ToString(),
            CreatedByName = bank.CreatedBy?.Name,
            UpdatedAt = bank.UpdatedAt,
            UpdatedById = bank.UpdatedById?.ToString(),
            UpdatedByName = bank.UpdatedBy?.Name,
        };
    }
}
