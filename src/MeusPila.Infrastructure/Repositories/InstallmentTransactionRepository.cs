using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeusPila.Infrastructure.Repositories;

public class InstallmentTransactionRepository
    : BaseRepository<InstallmentTransaction>,
        IInstallmentTransactionRepository
{
    public InstallmentTransactionRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    protected override IQueryable<InstallmentTransaction> GetDbSetRelations()
    {
        return _dbSet.Include(b => b.CreatedBy).Include(b => b.UpdatedBy);
    }
}
