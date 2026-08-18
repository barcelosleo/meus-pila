using System.Linq.Expressions;
using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.TestDoubles;

internal sealed class InMemoryHouseholdRepository : IGroupRepository
{
    private readonly List<Group> _households;

    public InMemoryHouseholdRepository(IEnumerable<Group>? seed = null)
    {
        _households = seed?.Select(EnsureId).ToList() ?? [];
    }

    public int AddCalls { get; private set; }
    public int UpdateCalls { get; private set; }
    public int AddMemberCalls { get; private set; }
    public int RemoveMemberCalls { get; private set; }

    public Task<Group?> GetByIdAsync(Guid id) =>
        Task.FromResult(_households.FirstOrDefault(household => household.Id == id));

    public Task<IEnumerable<Group>> GetAllAsync() => Task.FromResult(_households.AsEnumerable());

    public Task<IEnumerable<Group>> GetAllAsync(Expression<Func<Group, bool>> predicate) =>
        Task.FromResult(_households.AsQueryable().Where(predicate).AsEnumerable());

    public Task<IEnumerable<Group>> GetAllAsync(int pageNumber, int pageSize) =>
        Task.FromResult(_households.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable());

    public Task<IEnumerable<Group>> GetAllAsync(
        Expression<Func<Group, bool>> predicate,
        int pageNumber,
        int pageSize
    ) =>
        Task.FromResult(
            _households.AsQueryable().Where(predicate).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable()
        );

    public Task<Group?> FindAsync(Expression<Func<Group, bool>> predicate) =>
        Task.FromResult(_households.AsQueryable().FirstOrDefault(predicate));

    public Task<IEnumerable<Group>> FindAllAsync(Expression<Func<Group, bool>> predicate) =>
        Task.FromResult(_households.AsQueryable().Where(predicate).AsEnumerable());

    public Task<int> CountAsync() => Task.FromResult(_households.Count);

    public Task<int> CountAsync(Expression<Func<Group, bool>> predicate) =>
        Task.FromResult(_households.AsQueryable().Count(predicate));

    public Task<Group> AddAsync(Group entity)
    {
        AddCalls++;
        _households.Add(EnsureId(entity));
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<Group>> AddAsync(IEnumerable<Group> entities)
    {
        var list = entities.Select(EnsureId).ToList();
        AddCalls += list.Count;
        _households.AddRange(list);
        return Task.FromResult(list.AsEnumerable());
    }

    public Task<Group> UpdateAsync(Group entity)
    {
        UpdateCalls++;
        var index = _households.FindIndex(household => household.Id == entity.Id);
        if (index >= 0)
        {
            _households[index] = entity;
        }
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<Group>> UpdateAsync(IEnumerable<Group> entities)
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

    public Task DeleteAsync(Group entity)
    {
        _households.RemoveAll(household => household.Id == entity.Id);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IEnumerable<Group> entities)
    {
        foreach (var entity in entities)
        {
            _households.RemoveAll(household => household.Id == entity.Id);
        }
        return Task.CompletedTask;
    }

    public Task<bool> IsMemberOfGroupAsync(Group household, UserEntity user) =>
        Task.FromResult(household.Members.Any(member => member.UserId == user.Id));

    public Task AddMemberAsync(Group household, UserEntity user)
    {
        AddMemberCalls++;
        if (!household.Members.Any(member => member.UserId == user.Id))
        {
            household.Members.Add(
                new GroupUser
                {
                    GroupId = household.Id,
                    Group = household,
                    UserId = user.Id,
                    User = user,
                }
            );
        }
        return Task.CompletedTask;
    }

    public Task RemoveMemberAsync(Group household, UserEntity user)
    {
        RemoveMemberCalls++;
        household.Members = household.Members.Where(member => member.UserId != user.Id).ToList();
        return Task.CompletedTask;
    }

    public Task<IEnumerable<UserEntity>> GetMembers(Group household) =>
        Task.FromResult(household.Members.Select(member => member.User).AsEnumerable());

    private static Group EnsureId(Group household)
    {
        if (household.Id == Guid.Empty)
        {
            household.Id = Guid.NewGuid();
        }
        return household;
    }
}
