using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeusPila.Infrastructure.Repositories;

public class CompositeTransactionRepository : BaseRepository<CompositeTransaction>, ICompositeTransactionRepository
{
    public CompositeTransactionRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    protected override IQueryable<CompositeTransaction> GetDbSetRelations()
    {
        return _dbSet.Include(b => b.CreatedBy).Include(b => b.UpdatedBy);
    }
}
