using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using GroupEntity = MeusPila.Domain.Entities.Group;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Group.CreateGroup;

public interface ICreateGroupUseCase : IUseCase<CreateGroupInputDto, CreateGroupOutputDto> { }

public class CreateGroupUseCase : ICreateGroupUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _householdRepository;

    public CreateGroupUseCase(IUserRepository userRepository, IGroupRepository householdRepository)
    {
        _userRepository = userRepository;
        _householdRepository = householdRepository;
    }

    public async Task<CreateGroupOutputDto> ExecuteAsync(CreateGroupInputDto input)
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

        // If the owner is a regular user, promote them to GroupManager
        if (owner.Role == "User")
        {
            owner.Role = "GroupManager";
        }

        var group = new GroupEntity
        {
            Name = input.Name,
            OwnerId = owner.Id,
            Owner = owner,
            CreatedAt = DateTime.UtcNow,
            CreatedById = createdBy?.Id,
            CreatedBy = createdBy,
        };

        GroupEntity createdGroup = await _householdRepository.AddAsync(group);

        return new CreateGroupOutputDto
        {
            Id = createdGroup.Id.ToString(),
            Name = createdGroup.Name,
            OwnerId = createdGroup.OwnerId.ToString(),
            CreatedById = createdGroup.CreatedById?.ToString() ?? string.Empty,
            CreatedAt = createdGroup.CreatedAt,
        };
    }
}
