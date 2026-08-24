using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeusPila.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    protected override IQueryable<User> GetDbSetRelations()
    {
        return _dbSet.Include(b => b.CreatedBy).Include(b => b.UpdatedBy);
    }

    public async Task<bool> IsMemberOfGroupAsync(Guid userId, Guid householdId)
    {
        return await _dbContext.GroupUsers.AnyAsync(hu => hu.UserId == userId && hu.GroupId == householdId);
    }

    public async Task<bool> IsUserGroupManagerAsync(Guid userId, Guid householdId)
    {
        var householdUser = await _dbContext.GroupUsers.FirstOrDefaultAsync(hu =>
            hu.UserId == userId && hu.GroupId == householdId
        );

        return householdUser != null && householdUser.Group.OwnerId == userId;
    }

    public async Task<bool> UserCanViewGroupMembersAsync(Guid requesterId, Guid userId)
    {
        return await _dbContext.GroupUsers.AnyAsync(hu => hu.UserId == userId && hu.Group.OwnerId == requesterId);
    }

    public async Task<IEnumerable<Group>> GetGroupsByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null
    )
    {
        var query = _dbContext.GroupUsers.Where(hu => hu.UserId == userId).Select(hu => hu.Group).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(h => h.Name.Contains(searchTerm));
        }

        return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<int> CountGroupsByUserIdAsync(Guid userId, string? searchTerm = null)
    {
        var query = _dbContext.GroupUsers.Where(hu => hu.UserId == userId).Select(hu => hu.Group).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(h => h.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        return await query.CountAsync();
    }
}
