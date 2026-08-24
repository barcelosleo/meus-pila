using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeusPila.Infrastructure.Repositories;

public class FinancialCategoryRepository : BaseRepository<FinancialCategory>, IFinancialCategoryRepository
{
    public FinancialCategoryRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    protected override IQueryable<FinancialCategory> GetDbSetRelations()
    {
        return _dbSet.Include(b => b.CreatedBy).Include(b => b.UpdatedBy);
    }
}
