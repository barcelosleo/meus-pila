using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;

namespace MeusPila.Infrastructure.Repositories;

public class CompositeTransactionRepository : BaseRepository<CompositeTransaction>, ICompositeTransactionRepository
{
    public CompositeTransactionRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }
}
