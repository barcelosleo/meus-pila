using System.Linq.Expressions;
using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Infrastructure.Repositories;

public class GroupRepository : BaseRepository<Group>, IGroupRepository
{
    public GroupRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    protected override IQueryable<Group> GetDbSetRelations()
    {
        return _dbSet.Include(g => g.Owner).Include(b => b.CreatedBy).Include(b => b.UpdatedBy);
    }

    public async Task<bool> IsMemberOfGroupAsync(Group household, UserEntity user)
    {
        return await _dbContext.GroupUsers.AnyAsync(hu => hu.UserId == user.Id && hu.GroupId == household.Id);
    }

    public async Task AddMemberAsync(Group household, UserEntity user)
    {
        if (!await IsMemberOfGroupAsync(household, user))
        {
            GroupUser householdUser = new()
            {
                GroupId = household.Id,
                Group = household,
                UserId = user.Id,
                User = user,
            };

            await _dbContext.GroupUsers.AddAsync(householdUser);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RemoveMemberAsync(Group household, UserEntity user)
    {
        if (await IsMemberOfGroupAsync(household, user))
        {
            GroupUser? householdUser = await _dbContext.GroupUsers.FirstOrDefaultAsync(hu =>
                hu.UserId == user.Id && hu.GroupId == household.Id
            );

            _dbContext.GroupUsers.Remove(householdUser!);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<UserEntity>> GetMembers(Group household)
    {
        var members = _dbContext.GroupUsers.Where(hu => hu.GroupId == household.Id);
        var users = await _dbContext
            .GroupUsers.Where(hu => hu.GroupId == household.Id)
            .Select(hu => hu.User)
            .ToListAsync();
        return users;
    }
}
