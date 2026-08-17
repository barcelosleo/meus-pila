using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Houshold.UpdateHousehold;

public interface IUpdateHouseholdUseCase : IUseCase<UpdateHouseholdInputDto, UpdateHouseholdOutputDto> { }

public class UpdateHouseholdUseCase : IUpdateHouseholdUseCase
{
    private readonly IHouseholdRepository _householdRepository;
    private readonly IUserRepository _userRepository;

    public UpdateHouseholdUseCase(IHouseholdRepository householdRepository, IUserRepository userRepository)
    {
        _householdRepository = householdRepository;
        _userRepository = userRepository;
    }

    public async Task<UpdateHouseholdOutputDto> ExecuteAsync(UpdateHouseholdInputDto input)
    {
        Guid householdId = Guid.Parse(input.Id);
        Household? household = await _householdRepository.GetByIdAsync(householdId);

        if (household == null)
        {
            throw new ArgumentException($"O grupo com ID {input.Id} não foi encontrado.");
        }

        if (!string.IsNullOrEmpty(input.Name))
        {
            household.Name = input.Name;
        }

        if (!string.IsNullOrEmpty(input.UpdatedById))
        {
            Guid updatedById = Guid.Parse(input.UpdatedById);
            UserEntity? updatedBy = await _userRepository.GetByIdAsync(updatedById);
            if (updatedBy == null)
            {
                throw new ArgumentException(
                    $"O usuário que está criando o grupo com ID {input.UpdatedById} não foi encontrado."
                );
            }
            household.UpdatedBy = updatedBy;
            household.UpdatedById = updatedBy.Id;
        }

        household.UpdatedAt = DateTime.UtcNow;

        Household updatedHousehold = await _householdRepository.UpdateAsync(household);

        return new UpdateHouseholdOutputDto
        {
            Id = updatedHousehold.Id.ToString(),
            Name = updatedHousehold.Name,
            OwnerId = updatedHousehold.OwnerId.ToString(),
            UpdatedById = updatedHousehold.UpdatedById?.ToString() ?? "",
            UpdateAt = updatedHousehold.UpdatedAt,
        };
    }
}
