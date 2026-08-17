using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Houshold.RemoveMember;

public interface IRemoveMemberUseCase : IUseCase<RemoveMemberInputDto, RemoveMemberOutputDto> { }

public class RemoveMemberUseCase : IRemoveMemberUseCase
{
    private readonly IHouseholdRepository _householdRepository;
    private readonly IUserRepository _userRepository;

    public RemoveMemberUseCase(IHouseholdRepository householdRepository, IUserRepository userRepository)
    {
        _householdRepository = householdRepository;
        _userRepository = userRepository;
    }

    public async Task<RemoveMemberOutputDto> ExecuteAsync(RemoveMemberInputDto input)
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

        if (requester.Role != "Admin" && requester.Role != "HouseholdManager")
        {
            throw new UnauthorizedAccessException("Apenas administradores ou gerentes do grupo podem remover membros.");
        }

        if (requester.Role == "HouseholdManager" && household.OwnerId != requester.Id)
        {
            throw new UnauthorizedAccessException("Apenas o gerente do grupo pode remover membros.");
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

        await _householdRepository.RemoveMemberAsync(household, member);
        Household updatedHousehold = await _householdRepository.UpdateAsync(household);

        IEnumerable<UserEntity> members = await _householdRepository.GetMembers(household);

        return new RemoveMemberOutputDto
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
