using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;

namespace MeusPila.Infrastructure.Repositories;

public class FinancialTransactionRepository : BaseRepository<FinancialTransaction>, IFinancialTransactionRepository
{
    public FinancialTransactionRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }
}
