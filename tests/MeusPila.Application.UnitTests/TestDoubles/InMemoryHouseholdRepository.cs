using System.Linq.Expressions;
using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.TestDoubles;

internal sealed class InMemoryHouseholdRepository : IHouseholdRepository
{
    private readonly List<Household> _households;

    public InMemoryHouseholdRepository(IEnumerable<Household>? seed = null)
    {
        _households = seed?.Select(EnsureId).ToList() ?? [];
    }

    public int AddCalls { get; private set; }
    public int UpdateCalls { get; private set; }
    public int AddMemberCalls { get; private set; }
    public int RemoveMemberCalls { get; private set; }

    public Task<Household?> GetByIdAsync(Guid id) =>
        Task.FromResult(_households.FirstOrDefault(household => household.Id == id));

    public Task<IEnumerable<Household>> GetAllAsync() => Task.FromResult(_households.AsEnumerable());

    public Task<IEnumerable<Household>> GetAllAsync(Expression<Func<Household, bool>> predicate) =>
        Task.FromResult(_households.AsQueryable().Where(predicate).AsEnumerable());

    public Task<IEnumerable<Household>> GetAllAsync(int pageNumber, int pageSize) =>
        Task.FromResult(_households.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable());

    public Task<IEnumerable<Household>> GetAllAsync(
        Expression<Func<Household, bool>> predicate,
        int pageNumber,
        int pageSize
    ) =>
        Task.FromResult(
            _households.AsQueryable().Where(predicate).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable()
        );

    public Task<Household?> FindAsync(Expression<Func<Household, bool>> predicate) =>
        Task.FromResult(_households.AsQueryable().FirstOrDefault(predicate));

    public Task<IEnumerable<Household>> FindAllAsync(Expression<Func<Household, bool>> predicate) =>
        Task.FromResult(_households.AsQueryable().Where(predicate).AsEnumerable());

    public Task<int> CountAsync() => Task.FromResult(_households.Count);

    public Task<int> CountAsync(Expression<Func<Household, bool>> predicate) =>
        Task.FromResult(_households.AsQueryable().Count(predicate));

    public Task<Household> AddAsync(Household entity)
    {
        AddCalls++;
        _households.Add(EnsureId(entity));
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<Household>> AddAsync(IEnumerable<Household> entities)
    {
        var list = entities.Select(EnsureId).ToList();
        AddCalls += list.Count;
        _households.AddRange(list);
        return Task.FromResult(list.AsEnumerable());
    }

    public Task<Household> UpdateAsync(Household entity)
    {
        UpdateCalls++;
        var index = _households.FindIndex(household => household.Id == entity.Id);
        if (index >= 0)
        {
            _households[index] = entity;
        }
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<Household>> UpdateAsync(IEnumerable<Household> entities)
    {
        var list = entities.ToList();
        foreach (var entity in list)
        {
            var index = _households.FindIndex(household => household.Id == entity.Id);
            if (index >= 0)
            {
                _households[index] = entity;
            }
        }
        UpdateCalls += list.Count;
        return Task.FromResult(list.AsEnumerable());
    }

    public Task DeleteAsync(Household entity)
    {
        _households.RemoveAll(household => household.Id == entity.Id);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IEnumerable<Household> entities)
    {
        foreach (var entity in entities)
        {
            _households.RemoveAll(household => household.Id == entity.Id);
        }
        return Task.CompletedTask;
    }

    public Task<bool> IsMemberOfHouseholdAsync(Household household, UserEntity user) =>
        Task.FromResult(household.Members.Any(member => member.UserId == user.Id));

    public Task AddMemberAsync(Household household, UserEntity user)
    {
        AddMemberCalls++;
        if (!household.Members.Any(member => member.UserId == user.Id))
        {
            household.Members.Add(
                new HouseholdUser
                {
                    HouseholdId = household.Id,
                    Household = household,
                    UserId = user.Id,
                    User = user,
                }
            );
        }
        return Task.CompletedTask;
    }

    public Task RemoveMemberAsync(Household household, UserEntity user)
    {
        RemoveMemberCalls++;
        household.Members = household.Members.Where(member => member.UserId != user.Id).ToList();
        return Task.CompletedTask;
    }

    public Task<IEnumerable<UserEntity>> GetMembers(Household household) =>
        Task.FromResult(household.Members.Select(member => member.User).AsEnumerable());

    private static Household EnsureId(Household household)
    {
        if (household.Id == Guid.Empty)
        {
            household.Id = Guid.NewGuid();
        }
        return household;
    }
}
