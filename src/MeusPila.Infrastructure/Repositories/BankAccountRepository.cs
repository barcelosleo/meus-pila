using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeusPila.Infrastructure.Repositories;

public class BankAccountRepository : BaseRepository<BankAccount>, IBankAccountRepository
{
    public BankAccountRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    protected override IQueryable<BankAccount> GetDbSetRelations()
    {
        return _dbSet.Include(b => b.CreatedBy).Include(b => b.UpdatedBy);
    }
}
