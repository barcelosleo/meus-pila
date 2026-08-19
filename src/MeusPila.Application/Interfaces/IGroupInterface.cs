using MeusPila.Domain.Entities;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.Interfaces;

/// <summary>
/// Represents a repository interface for performing CRUD operations on Group entities.
/// </summary>
public interface IGroupRepository : IRepository<Group>
{
    public Task<bool> IsMemberOfGroupAsync(Group household, UserEntity user);
    public Task AddMemberAsync(Group household, UserEntity user);
    public Task RemoveMemberAsync(Group household, UserEntity user);
    public Task<IEnumerable<UserEntity>> GetMembers(Group household);
}
