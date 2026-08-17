using System.Linq.Expressions;
using MeusPila.Application.Interfaces;
using MeusPila.Domain.Common;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.User.ListUsers;

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
        if (input.PageSize <= 0 || input.PageSize > PagedListProperties.MaxPageSize)
        {
            throw new ArgumentException(
                $"Page size must be between 1 and {PagedListProperties.MaxPageSize}.",
                nameof(input.PageSize)
            );
        }
        string? searchTerm = input.SearchTerm?.Trim();
        string? normalizedSearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.ToLower();

        Expression<Func<UserEntity, bool>> predicate = user =>
            normalizedSearchTerm == null
            || user.Name.ToLower().Contains(normalizedSearchTerm)
            || user.Email.ToLower().Contains(normalizedSearchTerm);

        IEnumerable<UserEntity> users = await _userRepository.GetAllAsync(predicate, input.PageNumber, input.PageSize);
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
                CreatedAt = user.CreatedAt,
            }),
        };
    }
}
