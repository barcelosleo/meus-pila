using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Entities;

public abstract class FinancialAccount : IEntity, IFinancialAccount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    public required User Owner { get; set; }
    public IEnumerable<FinancialTransaction> Transactions { get; set; } = [];
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }

    [ForeignKey("UpdatedById")]
    public User? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }

    public decimal GetBalance()
    {
        return Transactions.Sum(t => t.GetValue());
    }

    public decimal GetBalance(Expression<Func<FinancialTransaction, bool>> filter)
    {
        var filteredTransactions = Transactions.AsQueryable().Where(filter);
        return filteredTransactions.Sum(t => t.GetValue(filter));
    }
}
