using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using GroupEntity = MeusPila.Domain.Entities.Group;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Group.UpdateGroup;

public interface IUpdateGroupUseCase : IUseCase<UpdateGroupInputDto, UpdateGroupOutputDto> { }

public class UpdateGroupUseCase : IUpdateGroupUseCase
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public UpdateGroupUseCase(IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<UpdateGroupOutputDto> ExecuteAsync(UpdateGroupInputDto input)
    {
        Guid groupId = Guid.Parse(input.Id);
        GroupEntity? group = await _groupRepository.GetByIdAsync(groupId);

        if (group == null)
        {
            throw new ArgumentException($"O grupo com ID {input.Id} não foi encontrado.");
        }

        if (!string.IsNullOrEmpty(input.OwnerId))
        {
            Guid ownerId = Guid.Parse(input.OwnerId);
            UserEntity? owner = await _userRepository.GetByIdAsync(ownerId);
            if (owner == null)
            {
                throw new ArgumentException($"O proprietário do grupo com ID {input.OwnerId} não foi encontrado.");
            }
            group.Owner = owner;
            group.OwnerId = owner.Id;
        }

        Guid updatedById = Guid.Parse(input.UpdatedById);
        UserEntity? updatedBy = await _userRepository.GetByIdAsync(updatedById);
        if (updatedBy == null)
        {
            throw new ArgumentException(
                $"O usuário que está criando o grupo com ID {input.UpdatedById} não foi encontrado."
            );
        }

        if (!string.IsNullOrEmpty(input.Name))
        {
            group.Name = input.Name;
        }

        group.UpdatedAt = DateTime.UtcNow;
        group.UpdatedBy = updatedBy;
        group.UpdatedById = updatedBy.Id;

        GroupEntity updatedGroup = await _groupRepository.UpdateAsync(group);

        return new UpdateGroupOutputDto
        {
            Id = updatedGroup.Id.ToString(),
            Name = updatedGroup.Name,
            OwnerId = updatedGroup.OwnerId.ToString(),
            UpdatedById = updatedGroup.UpdatedById?.ToString(),
            UpdatedAt = updatedGroup.UpdatedAt,
        };
    }
}
