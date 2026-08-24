using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;

namespace MeusPila.Infrastructure.Repositories;

public class FinancialCategoryRepository : BaseRepository<FinancialCategory>, IFinancialCategoryRepository
{
    public FinancialCategoryRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }
}
