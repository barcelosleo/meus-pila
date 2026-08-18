using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using GroupEntity = MeusPila.Domain.Entities.Group;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Group.RemoveMember;

public interface IRemoveMemberUseCase : IUseCase<RemoveMemberInputDto, RemoveMemberOutputDto> { }

public class RemoveMemberUseCase : IRemoveMemberUseCase
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public RemoveMemberUseCase(IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<RemoveMemberOutputDto> ExecuteAsync(RemoveMemberInputDto input)
    {
        Guid groupId = Guid.Parse(input.GroupId);
        GroupEntity? group = await _groupRepository.GetByIdAsync(groupId);

        if (group is null)
        {
            throw new ArgumentException($"O grupo com ID {input.GroupId} não foi encontrado.");
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

        if (requester.Role == "HouseholdManager" && group.OwnerId != requester.Id)
        {
            throw new UnauthorizedAccessException("Apenas o gerente do grupo pode remover membros.");
        }

        Guid memberId = Guid.Parse(input.MemberId);
        UserEntity? member = await _userRepository.GetByIdAsync(memberId);
        if (member is null)
        {
            throw new ArgumentException($"O usuário com ID {input.MemberId} não foi encontrado.");
        }

        group.UpdatedAt = DateTime.Now;
        group.UpdatedBy = requester;
        group.UpdatedById = requester.Id;

        await _groupRepository.RemoveMemberAsync(group, member);
        GroupEntity updatedGroup = await _groupRepository.UpdateAsync(group);

        IEnumerable<UserEntity> members = await _groupRepository.GetMembers(group);

        return new RemoveMemberOutputDto
        {
            Id = group.Id.ToString(),
            Name = group.Name,
            CreatedAt = group.CreatedAt,
            CreatedById = group.CreatedById?.ToString() ?? "",
            UpdatedAt = group.UpdatedAt,
            UpdatedById = group.UpdatedById?.ToString() ?? "",
            Members = [.. members.Select(m => new MemberDto { Id = m.Id.ToString(), Name = m.Name })],
        };
    }
}
