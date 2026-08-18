using MeusPila.Domain.Entities;

namespace MeusPila.Application.Interfaces;

/// <summary>
/// Represents a repository interface for performing CRUD operations on User entities.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    public Task<bool> IsMemberOfHouseholdAsync(Guid userId, Guid householdId);
    public Task<bool> IsUserHouseholdManagerAsync(Guid userId, Guid householdId);
    public Task<bool> UserCanViewHouseholdMembersAsync(Guid requesterId, Guid userId);
    public Task<IEnumerable<Household>> GetHouseholdsByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null
    );
    public Task<int> CountHouseholdsByUserIdAsync(Guid userId, string? searchTerm = null);
}
