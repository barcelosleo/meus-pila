using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;

namespace MeusPila.Infrastructure.Repositories;

public class BankRepository : BaseRepository<Bank>, IBankRepository
{
    public BankRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }
}
