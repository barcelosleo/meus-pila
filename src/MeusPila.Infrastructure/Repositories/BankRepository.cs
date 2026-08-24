using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeusPila.Infrastructure.Repositories;

public class BankRepository : BaseRepository<Bank>, IBankRepository
{
    public BankRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    protected override IQueryable<Bank> GetDbSetRelations()
    {
        return _dbSet.Include(b => b.CreatedBy).Include(b => b.UpdatedBy);
    }
}
