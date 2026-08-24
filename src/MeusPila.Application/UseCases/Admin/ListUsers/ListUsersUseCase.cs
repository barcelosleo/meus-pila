using System.Linq.Expressions;
using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using MeusPila.Domain.Common;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Admin.ListUsers;

public interface IListUsersUseCase : IUseCase<ListUsersInputDto, ListUsersOutputDto> { }

public class ListUsersUseCase : IListUsersUseCase
{
    private readonly IUserRepository _userRepository;

    public ListUsersUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ListUsersOutputDto> ExecuteAsync(ListUsersInputDto input)
    {
        string? searchTerm = input.SearchTerm?.Trim();
        string? normalizedSearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.ToLower();

        IEnumerable<UsersOrdering> inputOrdering = input.Ordering ?? [UsersOrdering.NameAsc];

        var ordering = QueryOrderingMapper<UserEntity, UsersOrdering>.MapQueryOrderings(inputOrdering);

        Expression<Func<UserEntity, bool>> predicate = user =>
            normalizedSearchTerm == null
            || user.Name.ToLower().Contains(normalizedSearchTerm)
            || user.Email.ToLower().Contains(normalizedSearchTerm);

        IEnumerable<UserEntity> users = await _userRepository.GetAllAsync(
            predicate,
            input.PageNumber,
            input.PageSize,
            ordering
        );
        int totalCount = await _userRepository.CountAsync(predicate);

        return new ListUsersOutputDto
        {
            PageNumber = input.PageNumber,
            PageSize = input.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)input.PageSize),
            Items = users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                CreatedById = user.CreatedById?.ToString(),
                CreatedByName = user.CreatedBy?.Name,
                UpdatedAt = user.UpdatedAt,
                UpdatedById = user.UpdatedById?.ToString(),
                UpdatedByName = user.UpdatedBy?.Name,
            }),
        };
    }
}
