using MeusPila.Application.Interfaces;
using MeusPila.Domain.Entities;
using MeusPila.Infrastructure.Data;

namespace MeusPila.Infrastructure.Repositories;

public class CreditCardRepository : BaseRepository<CreditCard>, ICreditCardRepository
{
    public CreditCardRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }
}
