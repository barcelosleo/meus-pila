using System.Linq.Expressions;
using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Houshold.ListHouseholds;

public interface IListHouseholdsUseCase : IUseCase<ListHouseholdsInputDto, ListHouseholdsOutputDto> { }

public class ListHouseholdsUseCase : IListHouseholdsUseCase
{
    private readonly IHouseholdRepository _householdRepository;
    private readonly IUserRepository _userRepository;

    public ListHouseholdsUseCase(IHouseholdRepository householdRepository, IUserRepository userRepository)
    {
        _householdRepository = householdRepository;
        _userRepository = userRepository;
    }

    public async Task<ListHouseholdsOutputDto> ExecuteAsync(ListHouseholdsInputDto input)
    {
        Guid requesterUserId = Guid.Parse(input.RequesterUserId);
        UserEntity? requesterUser = await _userRepository.GetByIdAsync(requesterUserId);
        if (requesterUser == null)
        {
            throw new ArgumentException("Usuário solicitante não encontrado.");
        }

        string? searchTerm = input.SearchTerm?.Trim();
        string? normalizedSearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.ToLower();

        IEnumerable<Household> households;
        int householdCount;

        if (requesterUser.Role == "Admin")
        {
            Expression<Func<Household, bool>>? filter = h =>
                normalizedSearchTerm == null || h.Name.ToLower().Contains(normalizedSearchTerm);
            households = await _householdRepository.GetAllAsync(filter, input.PageNumber, input.PageSize);
            householdCount = await _householdRepository.CountAsync(filter);
        }
        else
        {
            households = await _userRepository.GetHouseholdsByUserIdAsync(
                requesterUserId,
                input.PageNumber,
                input.PageSize,
                input.SearchTerm
            );
            householdCount = await _userRepository.CountHouseholdsByUserIdAsync(requesterUserId, input.SearchTerm);
        }

        return new ListHouseholdsOutputDto
        {
            PageNumber = input.PageNumber,
            PageSize = input.PageSize,
            TotalCount = householdCount,
            TotalPages = (int)Math.Ceiling((double)householdCount / input.PageSize),
            Items = households.Select(h => new HouseholdDto
            {
                Id = h.Id,
                Name = h.Name,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt,
            }),
        };
    }
}
