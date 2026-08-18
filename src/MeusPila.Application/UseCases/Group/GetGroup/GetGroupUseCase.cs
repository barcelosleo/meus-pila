using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using GroupEntity = MeusPila.Domain.Entities.Group;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Group.GetGroup;

public interface IGetGroupUseCase : IUseCase<GetGroupInputDto, GetGroupOutputDto> { }

public class GetGroupUseCase : IGetGroupUseCase
{
    private readonly IGroupRepository _groupRepository;

    public GetGroupUseCase(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GetGroupOutputDto> ExecuteAsync(GetGroupInputDto input)
    {
        Guid groupId = Guid.Parse(input.GroupId);
        GroupEntity? group = await _groupRepository.GetByIdAsync(groupId);

        if (group is null)
        {
            throw new ArgumentException($"O grupo com ID {input.GroupId} não foi encontrado.");
        }

        IEnumerable<UserEntity> members = await _groupRepository.GetMembers(group);

        return new GetGroupOutputDto
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
