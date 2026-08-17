using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Infrastructure.Repositories;

public class HouseholdRepository : BaseRepository<Household>, IHouseholdRepository
{
    public HouseholdRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    public async Task<bool> IsMemberOfHouseholdAsync(Household household, UserEntity user)
    {
        return await _dbContext.HouseholdUsers.AnyAsync(hu => hu.UserId == user.Id && hu.HouseholdId == household.Id);
    }

    public async Task AddMemberAsync(Household household, UserEntity user)
    {
        if (!await IsMemberOfHouseholdAsync(household, user))
        {
            HouseholdUser householdUser = new()
            {
                HouseholdId = household.Id,
                Household = household,
                UserId = user.Id,
                User = user,
            };

            await _dbContext.HouseholdUsers.AddAsync(householdUser);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RemoveMemberAsync(Household household, UserEntity user)
    {
        if (await IsMemberOfHouseholdAsync(household, user))
        {
            HouseholdUser? householdUser = await _dbContext.HouseholdUsers.FirstOrDefaultAsync(hu =>
                hu.UserId == user.Id && hu.HouseholdId == household.Id
            );

            _dbContext.HouseholdUsers.Remove(householdUser!);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<UserEntity>> GetMembers(Household household)
    {
        var members = _dbContext.HouseholdUsers.Where(hu => hu.HouseholdId == household.Id);
        var users = await _dbContext
            .Users.Join(members, u => u.Id, hu => hu.UserId, (u, hu) => new { User = u, HouseholdUser = hu })
            .Select(u => u.User)
            .ToListAsync();
        return users;
    }
}
