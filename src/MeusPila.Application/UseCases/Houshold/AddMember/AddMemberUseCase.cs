using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Houshold.AddMember;

public interface IAddMemberUseCase : IUseCase<AddMemberInputDto, AddMemberOutputDto> { }

public class AddMemberUseCase : IAddMemberUseCase
{
    private readonly IHouseholdRepository _householdRepository;
    private readonly IUserRepository _userRepository;

    public AddMemberUseCase(IHouseholdRepository householdRepository, IUserRepository userRepository)
    {
        _householdRepository = householdRepository;
        _userRepository = userRepository;
    }

    public async Task<AddMemberOutputDto> ExecuteAsync(AddMemberInputDto input)
    {
        Guid householdId = Guid.Parse(input.HouseholdId);
        Household? household = await _householdRepository.GetByIdAsync(householdId);

        if (household is null)
        {
            throw new ArgumentException($"O grupo com ID {input.HouseholdId} não foi encontrado.");
        }

        Guid requesterId = Guid.Parse(input.UpdatedById);
        UserEntity? requester = await _userRepository.GetByIdAsync(requesterId);
        if (requester is null)
        {
            throw new ArgumentException($"O usuário com ID {input.UpdatedById} não foi encontrado.");
        }

        Guid memberId = Guid.Parse(input.MemberId);
        UserEntity? member = await _userRepository.GetByIdAsync(memberId);
        if (member is null)
        {
            throw new ArgumentException($"O usuário com ID {input.MemberId} não foi encontrado.");
        }

        household.UpdatedAt = DateTime.Now;
        household.UpdatedBy = requester;
        household.UpdatedById = requester.Id;

        await _householdRepository.AddMemberAsync(household, member);
        Household updatedHousehold = await _householdRepository.UpdateAsync(household);

        IEnumerable<UserEntity> members = await _householdRepository.GetMembers(updatedHousehold);

        return new AddMemberOutputDto
        {
            HouseholdId = updatedHousehold.Id.ToString(),
            Name = updatedHousehold.Name,
            UpdatedById = updatedHousehold.UpdatedById?.ToString() ?? "",
            UpdatedAt = updatedHousehold.UpdatedAt,
            Members = [.. members.Select(m => new MemberDto { Id = m.Id.ToString(), Name = m.Name })],
        };
    }
}
