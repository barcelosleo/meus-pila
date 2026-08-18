using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using GroupEntity = MeusPila.Domain.Entities.Group;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Group.AddMember;

public interface IAddMemberUseCase : IUseCase<AddMemberInputDto, AddMemberOutputDto> { }

public class AddMemberUseCase : IAddMemberUseCase
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public AddMemberUseCase(IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<AddMemberOutputDto> ExecuteAsync(AddMemberInputDto input)
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

        Guid memberId = Guid.Parse(input.MemberId);
        UserEntity? member = await _userRepository.GetByIdAsync(memberId);
        if (member is null)
        {
            throw new ArgumentException($"O usuário com ID {input.MemberId} não foi encontrado.");
        }

        group.UpdatedAt = DateTime.Now;
        group.UpdatedBy = requester;
        group.UpdatedById = requester.Id;

        await _groupRepository.AddMemberAsync(group, member);
        GroupEntity updatedHousehold = await _groupRepository.UpdateAsync(group);

        IEnumerable<UserEntity> members = await _groupRepository.GetMembers(updatedHousehold);

        return new AddMemberOutputDto
        {
            GroupId = updatedHousehold.Id.ToString(),
            Name = updatedHousehold.Name,
            UpdatedById = updatedHousehold.UpdatedById?.ToString() ?? "",
            UpdatedAt = updatedHousehold.UpdatedAt,
            Members = [.. members.Select(m => new MemberDto { Id = m.Id.ToString(), Name = m.Name })],
        };
    }
}
