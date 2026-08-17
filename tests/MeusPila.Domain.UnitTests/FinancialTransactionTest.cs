using MeusPila.Domain.Entities;
using MeusPila.Domain.Enums;

namespace MeusPila.Domain.UnitTests;

public class FinancialTransactionTest
{
    [Fact]
    public void Should_Create_FinancialTransaction()
    {
        // Arrange
        var value = 100m;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var type = TransactionType.Inflow;
        var category = CreateCategory("Salary");

        // Act
        var transaction = CreateTransaction(value, date, type, category);

        // Assert
        Assert.Equal(value, transaction.GetValue());
        Assert.Equal(date, transaction.Date);
        Assert.Equal(type, transaction.Type);
        Assert.Equal(category, transaction.Category);
    }

    [Fact]
    public void Should_Not_Allow_Negative_Value()
    {
        // Arrange
        var value = -100m;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var type = TransactionType.Inflow;
        var category = CreateCategory("Salary");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateTransaction(value, date, type, category));
    }

    private static FinancialCategory CreateCategory(string name)
    {
        return new FinancialCategory { Name = name };
    }

    private static IndividualTransaction CreateTransaction(
        decimal value,
        DateOnly date,
        TransactionType type,
        FinancialCategory category
    )
    {
        var transaction = new IndividualTransaction
        {
            Date = date,
            Performed = false,
            Type = type,
            Category = category,
            CategoryId = category.Id,
        };

        transaction.SetValue(value);

        return transaction;
    }
}
