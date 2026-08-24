using MeusPila.Application.Interfaces;
using BankEntity = MeusPila.Domain.Entities.Bank;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Bank.RemoveBank;

public interface IRemoveBankUseCase : IUseCase<RemoveBankInputDto, object> { }

public class RemoveBankUseCase : IRemoveBankUseCase
{
    private readonly IBankRepository _bankRepository;
    private readonly IUserRepository _userRepository;

    public RemoveBankUseCase(IBankRepository bankRepository, IUserRepository userRepository)
    {
        _bankRepository = bankRepository;
        _userRepository = userRepository;
    }

    public async Task<object> ExecuteAsync(RemoveBankInputDto input)
    {
        Guid bankId = Guid.Parse(input.Id);
        BankEntity? bank = await _bankRepository.GetByIdAsync(bankId);
        if (bank is null)
        {
            throw new ArgumentException($"Banco com ID '{input.Id}' não encontrado.");
        }

        Guid requesterId = Guid.Parse(input.RequesterId);
        UserEntity? requester = await _userRepository.GetByIdAsync(requesterId);
        if (requester is null)
        {
            throw new ArgumentException($"Usuário com ID '{input.RequesterId}' não encontrado.");
        }

        if (requester.Role != "Admin")
        {
            throw new UnauthorizedAccessException("O usuário não tem permissão para remover um banco.");
        }

        await _bankRepository.DeleteAsync(bank);
        return null!;
    }
}
