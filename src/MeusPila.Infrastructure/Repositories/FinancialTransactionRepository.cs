using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeusPila.Infrastructure.Repositories;

public class FinancialTransactionRepository : BaseRepository<FinancialTransaction>, IFinancialTransactionRepository
{
    public FinancialTransactionRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    protected override IQueryable<FinancialTransaction> GetDbSetRelations()
    {
        return _dbSet.Include(b => b.CreatedBy).Include(b => b.UpdatedBy);
    }
}
