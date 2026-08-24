using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;

namespace MeusPila.Infrastructure.Repositories;

public class InstallmentTransactionRepository
    : BaseRepository<InstallmentTransaction>,
        IInstallmentTransactionRepository
{
    public InstallmentTransactionRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }
}
