using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;

namespace MeusPila.Infrastructure.Repositories;

public class BankAccountRepository : BaseRepository<BankAccount>, IBankAccountRepository
{
    public BankAccountRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }
}
