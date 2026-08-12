using System.Linq.Expressions;

namespace MeusPila.Domain.Interfaces;

public interface IFinancialAccount
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid OwnerId { get; }
    public List<IFinancialTransaction> Transactions { get; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public decimal GetBalance();
    public decimal GetBalance(Expression<Func<IFinancialTransaction, bool>> filter);
}
