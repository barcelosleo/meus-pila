using MeusPila.Domain.Entities;

namespace MeusPila.Application.Interfaces;

/// <summary>
/// Represents a repository interface for performing CRUD operations on User entities.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    public Task<bool> IsMemberOfGroupAsync(Guid userId, Guid householdId);
    public Task<bool> IsUserGroupManagerAsync(Guid userId, Guid householdId);
    public Task<bool> UserCanViewGroupMembersAsync(Guid requesterId, Guid userId);
    public Task<IEnumerable<Group>> GetGroupsByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null
    );
    public Task<int> CountGroupsByUserIdAsync(Guid userId, string? searchTerm = null);
}
