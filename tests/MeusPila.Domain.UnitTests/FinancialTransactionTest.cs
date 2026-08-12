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
        var category = new FinancialCategory("Salary");

        // Act
        var transaction = new FinancialTransaction(value, date, type, category);

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
        var category = new FinancialCategory("Salary");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new FinancialTransaction(value, date, type, category));
    }
}
