using MeusPila.Application.Interfaces;
using BankEntity = MeusPila.Domain.Entities.Bank;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Bank.UpdateBank;

public interface IUpdateBankUseCase : IUseCase<UpdateBankInputDto, UpdateBankOutputDto> { }

public class UpdateBankUseCase : IUpdateBankUseCase
{
    private readonly IBankRepository _bankRepository;
    private readonly IUserRepository _userRepository;

    public UpdateBankUseCase(IBankRepository bankRepository, IUserRepository userRepository)
    {
        _bankRepository = bankRepository;
        _userRepository = userRepository;
    }

    public async Task<UpdateBankOutputDto> ExecuteAsync(UpdateBankInputDto input)
    {
        Guid bankId = Guid.Parse(input.Id);
        BankEntity? bank = await _bankRepository.GetByIdAsync(bankId);
        if (bank is null)
        {
            throw new ArgumentException($"Banco com ID '{input.Id}' não encontrado.");
        }

        Guid updatedById = Guid.Parse(input.UpdatedById);
        UserEntity? updatedBy = await _userRepository.GetByIdAsync(updatedById);
        if (updatedBy is null)
        {
            throw new ArgumentException($"Usuário com ID '{input.UpdatedById}' não encontrado.");
        }

        if (!string.IsNullOrWhiteSpace(input.Name))
        {
            bank.Name = input.Name;
        }
        if (!string.IsNullOrWhiteSpace(input.Code))
        {
            if ((await _bankRepository.CountAsync(b => b.Code == input.Code && b.Id != bankId)) > 0)
            {
                throw new ArgumentException($"Código '{input.Code}' já está em uso por outro banco.");
            }
            bank.Code = input.Code;
        }

        bank.UpdatedAt = DateTime.UtcNow;
        bank.UpdatedById = updatedById;
        bank.UpdatedBy = updatedBy;

        BankEntity updatedBank = await _bankRepository.UpdateAsync(bank);

        return new UpdateBankOutputDto
        {
            Id = updatedBank.Id.ToString(),
            Name = updatedBank.Name,
            Code = updatedBank.Code,
            CreatedAt = updatedBank.CreatedAt,
            CreatedById = updatedBank.CreatedById?.ToString(),
            CreatedByName = updatedBank.CreatedBy?.Name,
            UpdatedAt = updatedBank.UpdatedAt,
            UpdatedById = updatedBank.UpdatedById?.ToString(),
            UpdatedByName = updatedBank.UpdatedBy?.Name,
        };
    }
}
