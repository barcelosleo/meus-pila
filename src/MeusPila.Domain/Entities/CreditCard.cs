namespace MeusPila.Domain.Entities;

public class CreditCard : FinancialAccount
{
    public required DateOnly ClosingDate { get; set; }
    public required DateOnly DueDate { get; set; }
    public required decimal Limit
    {
        get => _limit;
        set => SetLimit(value);
    }
    private decimal _limit;

    public void SetLimit(decimal limit)
    {
        if (limit < 0)
        {
            throw new ArgumentException("Limit cannot be negative.", nameof(limit));
        }
        _limit = limit;
    }
}
