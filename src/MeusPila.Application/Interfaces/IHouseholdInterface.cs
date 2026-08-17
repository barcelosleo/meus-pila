using MeusPila.Domain.Entities;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.Interfaces;

/// <summary>
/// Represents a repository interface for performing CRUD operations on Household entities.
/// </summary>
public interface IHouseholdRepository : IRepository<Household>
{
    public Task<bool> IsMemberOfHouseholdAsync(Household household, UserEntity user);
    public Task AddMemberAsync(Household household, UserEntity user);
    public Task RemoveMemberAsync(Household household, UserEntity user);
    public Task<IEnumerable<UserEntity>> GetMembers(Household household);
}
