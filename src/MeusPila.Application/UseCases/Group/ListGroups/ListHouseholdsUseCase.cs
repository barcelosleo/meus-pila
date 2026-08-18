using System.Linq.Expressions;
using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using GroupEntity = MeusPila.Domain.Entities.Group;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Group.ListGroups;

public interface IListGroupsUseCase : IUseCase<ListGroupsInputDto, ListGroupsOutputDto> { }

public class ListGroupsUseCase : IListGroupsUseCase
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public ListGroupsUseCase(IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<ListGroupsOutputDto> ExecuteAsync(ListGroupsInputDto input)
    {
        Guid requesterUserId = Guid.Parse(input.RequesterUserId);
        UserEntity? requesterUser = await _userRepository.GetByIdAsync(requesterUserId);
        if (requesterUser == null)
        {
            throw new ArgumentException("Usuário solicitante não encontrado.");
        }

        string? searchTerm = input.SearchTerm?.Trim();
        string? normalizedSearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.ToLower();

        IEnumerable<GroupEntity> groups;
        int groupCount;

        if (requesterUser.Role == "Admin")
        {
            Expression<Func<GroupEntity, bool>>? filter = h =>
                normalizedSearchTerm == null || h.Name.ToLower().Contains(normalizedSearchTerm);
            groups = await _groupRepository.GetAllAsync(filter, input.PageNumber, input.PageSize);
            groupCount = await _groupRepository.CountAsync(filter);
        }
        else
        {
            groups = await _userRepository.GetGroupsByUserIdAsync(
                requesterUserId,
                input.PageNumber,
                input.PageSize,
                input.SearchTerm
            );
            groupCount = await _userRepository.CountGroupsByUserIdAsync(requesterUserId, input.SearchTerm);
        }

        return new ListGroupsOutputDto
        {
            PageNumber = input.PageNumber,
            PageSize = input.PageSize,
            TotalCount = groupCount,
            TotalPages = (int)Math.Ceiling((double)groupCount / input.PageSize),
            Items = groups.Select(h => new GroupDto
            {
                Id = h.Id,
                Name = h.Name,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt,
            }),
        };
    }
}
