using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Houshold.CreateHousehold;

public interface ICreateHouseholdUseCase : IUseCase<CreateHouseholdInputDto, CreateHouseholdOutputDto> { }

public class CreateHouseholdUseCase : ICreateHouseholdUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IHouseholdRepository _householdRepository;

    public CreateHouseholdUseCase(IUserRepository userRepository, IHouseholdRepository householdRepository)
    {
        _userRepository = userRepository;
        _householdRepository = householdRepository;
    }

    public async Task<CreateHouseholdOutputDto> ExecuteAsync(CreateHouseholdInputDto input)
    {
        Guid ownerId = Guid.Parse(input.OwnerId);
        UserEntity? owner = await _userRepository.GetByIdAsync(ownerId);
        if (owner == null)
        {
            throw new ArgumentException($"O proprietário com ID {input.OwnerId} não foi encontrado.");
        }

        UserEntity? createdBy = null;
        if (!string.IsNullOrEmpty(input.CreatedById))
        {
            Guid createdById = Guid.Parse(input.CreatedById);
            createdBy = await _userRepository.GetByIdAsync(createdById);
            if (createdBy == null)
            {
                throw new ArgumentException(
                    $"O usuário que está criando o grupo com ID {input.CreatedById} não foi encontrado."
                );
            }
        }

        // If the owner is a regular user, promote them to HouseholdManager
        if (owner.Role == "User")
        {
            owner.Role = "HouseholdManager";
        }

        var household = new Household
        {
            Name = input.Name,
            OwnerId = owner.Id,
            Owner = owner,
            CreatedAt = DateTime.UtcNow,
            CreatedById = createdBy?.Id,
            CreatedBy = createdBy,
        };

        Household createdHousehold = await _householdRepository.AddAsync(household);

        return new CreateHouseholdOutputDto
        {
            Id = createdHousehold.Id.ToString(),
            Name = createdHousehold.Name,
            OwnerId = createdHousehold.OwnerId.ToString(),
            CreatedById = createdHousehold.CreatedById?.ToString() ?? string.Empty,
            CreatedAt = createdHousehold.CreatedAt,
        };
    }
}
