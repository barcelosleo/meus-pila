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
}
