using MeusPila.Application.Interfaces;

namespace MeusPila.Application.UseCases.Group.RemoveGroup;

public interface IRemoveGroupUseCase : IUseCase<RemoveGroupInputDto, object> { }

public class RemoveGroupUseCase : IRemoveGroupUseCase
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public RemoveGroupUseCase(IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<object> ExecuteAsync(RemoveGroupInputDto input)
    {
        Guid groupId = Guid.Parse(input.GroupId);
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new ArgumentException($"O grupo com ID {input.GroupId} não foi encontrado.");
        }

        Guid requesterId = Guid.Parse(input.RequesterId);
        var requester = await _userRepository.GetByIdAsync(requesterId);
        if (requester == null)
        {
            throw new ArgumentException(
                $"O usuário que está fazendo a requisição com ID {input.RequesterId} não foi encontrado."
            );
        }

        if (requester.Role != "Admin" && requester.Role != "GroupManager")
        {
            throw new UnauthorizedAccessException("Apenas administradores ou gerentes do grupo podem remover o grupo.");
        }

        if (requester.Role == "GroupManager" && group.OwnerId != requester.Id)
        {
            throw new UnauthorizedAccessException("Apenas o gerente do grupo pode remover o grupo.");
        }

        await _groupRepository.DeleteAsync(group);

        return null!;
    }
}
