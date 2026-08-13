using System.Linq.Expressions;
using MeusPila.Application.Interfaces;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.TestDoubles;

internal sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<UserEntity> _users;

    public InMemoryUserRepository(IEnumerable<UserEntity>? seed = null)
    {
        _users = seed?.ToList() ?? [];
    }

    public int UpdateCalls { get; private set; }
    public int AddCalls { get; private set; }

    public Task<UserEntity> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_users.FirstOrDefault(u => u.Id == id)!);
    }

    public Task<IEnumerable<UserEntity>> GetAllAsync()
    {
        return Task.FromResult(_users.AsEnumerable());
    }

    public Task<IEnumerable<UserEntity>> GetAllAsync(Expression<Func<UserEntity, bool>> predicate)
    {
        return Task.FromResult(_users.AsQueryable().Where(predicate).AsEnumerable());
    }

    public Task<IEnumerable<UserEntity>> GetAllAsync(int pageNumber, int pageSize)
    {
        return Task.FromResult(_users.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable());
    }

    public Task<IEnumerable<UserEntity>> GetAllAsync(
        Expression<Func<UserEntity, bool>> predicate,
        int pageNumber,
        int pageSize
    )
    {
        return Task.FromResult(
            _users.AsQueryable().Where(predicate).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable()
        );
    }

    public Task<UserEntity> FindAsync(Expression<Func<UserEntity, bool>> predicate)
    {
        return Task.FromResult(_users.AsQueryable().FirstOrDefault(predicate)!);
    }

    public Task<IEnumerable<UserEntity>> FindAllAsync(Expression<Func<UserEntity, bool>> predicate)
    {
        return Task.FromResult(_users.AsQueryable().Where(predicate).AsEnumerable());
    }

    public Task<int> CountAsync()
    {
        return Task.FromResult(_users.Count);
    }

    public Task<int> CountAsync(Expression<Func<UserEntity, bool>> predicate)
    {
        return Task.FromResult(_users.AsQueryable().Count(predicate));
    }

    public Task<UserEntity> AddAsync(UserEntity entity)
    {
        AddCalls++;
        _users.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<UserEntity>> AddAsync(IEnumerable<UserEntity> entities)
    {
        var list = entities.ToList();
        AddCalls += list.Count;
        _users.AddRange(list);
        return Task.FromResult(list.AsEnumerable());
    }

    public Task<UserEntity> UpdateAsync(UserEntity entity)
    {
        UpdateCalls++;
        var index = _users.FindIndex(u => u.Id == entity.Id);
        if (index >= 0)
        {
            _users[index] = entity;
        }
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<UserEntity>> UpdateAsync(IEnumerable<UserEntity> entities)
    {
        var list = entities.ToList();
        foreach (var entity in list)
        {
            var index = _users.FindIndex(u => u.Id == entity.Id);
            if (index >= 0)
            {
                _users[index] = entity;
            }
        }
        UpdateCalls += list.Count;
        return Task.FromResult(list.AsEnumerable());
    }

    public Task DeleteAsync(UserEntity entity)
    {
        _users.RemoveAll(u => u.Id == entity.Id);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IEnumerable<UserEntity> entities)
    {
        foreach (var entity in entities)
        {
            _users.RemoveAll(u => u.Id == entity.Id);
        }
        return Task.CompletedTask;
    }
}
