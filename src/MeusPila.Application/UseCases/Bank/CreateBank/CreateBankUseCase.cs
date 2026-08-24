using MeusPila.Application.Interfaces;
using BankEntity = MeusPila.Domain.Entities.Bank;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Bank.CreateBank;

public interface ICreateBankUseCase : IUseCase<CreateBankInputDto, CreateBankOutputDto> { }

public class CreateBankUseCase : ICreateBankUseCase
{
    private readonly IBankRepository _bankRepository;
    private readonly IUserRepository _userRepository;

    public CreateBankUseCase(IBankRepository bankRepository, IUserRepository userRepository)
    {
        _bankRepository = bankRepository;
        _userRepository = userRepository;
    }

    public async Task<CreateBankOutputDto> ExecuteAsync(CreateBankInputDto input)
    {
        Guid createdById = Guid.Parse(input.CreatedById);
        UserEntity? createdBy = await _userRepository.GetByIdAsync(createdById);
        if (createdBy == null)
        {
            throw new ArgumentException($"Usuário com ID {createdById} não encontrado.");
        }

        if (createdBy.Role != "Admin")
        {
            throw new UnauthorizedAccessException("Apenas usuários com a função 'Admin' podem criar bancos.");
        }

        if (await _bankRepository.CountAsync(b => b.Code == input.Code || b.Name == input.Name) > 0)
        {
            throw new ArgumentException($"Um banco com o código '{input.Code}' ou nome '{input.Name}' já existe.");
        }

        var bank = new BankEntity
        {
            Name = input.Name,
            Code = input.Code,
            CreatedById = createdById,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
        };

        BankEntity createdBank = await _bankRepository.AddAsync(bank);
        return new CreateBankOutputDto
        {
            Id = createdBank.Id.ToString(),
            Name = createdBank.Name,
            Code = createdBank.Code,
            CreatedAt = createdBank.CreatedAt,
        };
    }
}
