using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Houshold.GetHousehold;

public interface IGetHouseholdUseCase : IUseCase<GetHouseholdInputDto, GetHouseholdOutputDto> { }

public class GetHouseholdUseCase : IGetHouseholdUseCase
{
    private readonly IHouseholdRepository _householdRepository;

    public GetHouseholdUseCase(IHouseholdRepository householdRepository)
    {
        _householdRepository = householdRepository;
    }

    public async Task<GetHouseholdOutputDto> ExecuteAsync(GetHouseholdInputDto input)
    {
        Guid householdId = Guid.Parse(input.HouseholdId);
        Household? household = await _householdRepository.GetByIdAsync(householdId);

        if (household is null)
        {
            throw new ArgumentException($"O grupo com ID {input.HouseholdId} não foi encontrado.");
        }

        IEnumerable<UserEntity> members = await _householdRepository.GetMembers(household);

        return new GetHouseholdOutputDto
        {
            Id = household.Id.ToString(),
            Name = household.Name,
            CreatedAt = household.CreatedAt,
            CreatedById = household.CreatedById?.ToString() ?? "",
            UpdatedAt = household.UpdatedAt,
            UpdatedById = household.UpdatedById?.ToString() ?? "",
            Members = [.. members.Select(m => new MemberDto { Id = m.Id.ToString(), Name = m.Name })],
        };
    }
}
