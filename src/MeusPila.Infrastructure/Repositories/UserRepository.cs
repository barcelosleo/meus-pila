using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeusPila.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    public async Task<bool> IsMemberOfHouseholdAsync(Guid userId, Guid householdId)
    {
        return await _dbContext.HouseholdUsers.AnyAsync(hu => hu.UserId == userId && hu.HouseholdId == householdId);
    }

    public async Task<bool> IsUserHouseholdManagerAsync(Guid userId, Guid householdId)
    {
        var householdUser = await _dbContext.HouseholdUsers.FirstOrDefaultAsync(hu =>
            hu.UserId == userId && hu.HouseholdId == householdId
        );

        return householdUser != null && householdUser.Household.OwnerId == userId;
    }

    public async Task<bool> UserCanViewHouseholdMembersAsync(Guid requesterId, Guid userId)
    {
        return await _dbContext.HouseholdUsers.AnyAsync(hu =>
            hu.UserId == userId && hu.Household.OwnerId == requesterId
        );
    }

    public async Task<IEnumerable<Household>> GetHouseholdsByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null
    )
    {
        var query = _dbContext.HouseholdUsers.Where(hu => hu.UserId == userId).Select(hu => hu.Household).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(h => h.Name.Contains(searchTerm));
        }

        return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<int> CountHouseholdsByUserIdAsync(Guid userId, string? searchTerm = null)
    {
        var query = _dbContext.HouseholdUsers.Where(hu => hu.UserId == userId).Select(hu => hu.Household).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(h => h.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        return await query.CountAsync();
    }
}
