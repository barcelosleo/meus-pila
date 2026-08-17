using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using MeusPila.Domain.Enums;
using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Entities;

public abstract class FinancialTransaction : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public required DateOnly Date { get; set; }
    public required bool Performed { get; set; }
    public required TransactionType Type { get; set; }
    public required Guid CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public required FinancialCategory Category { get; set; }
    public Guid? ParentTransactionId { get; set; }

    [ForeignKey("ParentTransactionId")]
    public virtual FinancialTransaction? ParentTransaction { get; set; }

    public decimal Value
    {
        get => GetValue();
        set => SetValue(value);
    }
    protected decimal _value;
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }

    [ForeignKey("UpdatedById")]
    public User? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }

    public abstract void SetValue(decimal value);

    public abstract decimal GetValue();

    public abstract decimal GetValue(Expression<Func<FinancialTransaction, bool>> filter);
}
