using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using MeusPila.Domain.Entities;
using MeusPila.Domain.Enums;

namespace MeusPila.Domain.UnitTests;

public class CompositeFinancialTransactionTest
{
    private const string SalaryCategoryName = "Salary";

    [Theory]
    [InlineData(100, TransactionType.Inflow, 50, TransactionType.Inflow, 150, TransactionType.Inflow)]
    [InlineData(200, TransactionType.Inflow, 100, TransactionType.Outflow, 100, TransactionType.Inflow)]
    [InlineData(100, TransactionType.Outflow, 50, TransactionType.Outflow, -150, TransactionType.Outflow)]
    [InlineData(200, TransactionType.Outflow, 100, TransactionType.Inflow, -100, TransactionType.Outflow)]
    public void Should_Create_CompositeFinancialTransaction(
        decimal value1,
        TransactionType type1,
        decimal value2,
        TransactionType type2,
        decimal expectedNetValue,
        TransactionType expectedType
    )
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var category = CreateCategory(SalaryCategoryName);
        var childTransaction1 = CreateTransaction(value1, date, type1, category);
        var childTransaction2 = CreateTransaction(value2, date, type2, category);

        // Act
        var compositeTransaction = CreateCompositeWithChildren(date, category, childTransaction1, childTransaction2);

        // Assert
        Assert.Equal(date, compositeTransaction.Date);
        Assert.Equal(category, compositeTransaction.Category);
        Assert.Equal(2, compositeTransaction.ChildTransactions.Count);
        Assert.Equal(expectedType, compositeTransaction.Type);
        Assert.Equal(expectedNetValue, compositeTransaction.GetValue());
    }

    [Fact]
    public void Should_Filter_ChildTransactions_By_Date_And_Type()
    {
        // Arrange
        var date1 = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5));
        var date2 = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var date3 = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
        var category = CreateCategory(SalaryCategoryName);
        var childTransaction1 = CreateTransaction(100, date1, TransactionType.Inflow, category);
        var childTransaction2 = CreateTransaction(50, date2, TransactionType.Outflow, category);
        var childTransaction3 = CreateTransaction(500, date3, TransactionType.Inflow, category);

        // Act
        var compositeTransaction = CreateCompositeWithChildren(
            date1,
            category,
            childTransaction1,
            childTransaction2,
            childTransaction3
        );
        var filteredTransactions1 = compositeTransaction.GetValue(t =>
            t.Date >= date2 && t.Date <= date3 && t.Type == TransactionType.Inflow
        );
        var filteredTransactions2 = compositeTransaction.GetValue(t =>
            t.Date >= date1 && t.Date <= date2 && t.Type == TransactionType.Outflow
        );
        var filteredTransactions3 = compositeTransaction.GetValue(t => t.Date >= date1 && t.Date <= date3);

        // Assert
        Assert.Equal(500, filteredTransactions1);
        Assert.Equal(-50, filteredTransactions2);
        Assert.Equal(550, filteredTransactions3);
    }

    [Fact]
    public void Should_Throw_Exception_When_ChildTransactions_Have_Different_Categories()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var category1 = CreateCategory(SalaryCategoryName);
        var category2 = CreateCategory("Investment");
        var childTransaction1 = CreateTransaction(100, date, TransactionType.Inflow, category1);
        var childTransaction2 = CreateTransaction(50, date, TransactionType.Inflow, category2);

        // Act
        var compositeTransaction = CreateCompositeWithChildren(date, category1, childTransaction1);

        // Assert
        Assert.Throws<InvalidOperationException>(() => compositeTransaction.AddChildTransaction(childTransaction2));
        Assert.Throws<InvalidOperationException>(() => compositeTransaction.AddChildTransactions([childTransaction2]));
    }

    [Fact]
    public void Should_Throw_Exception_When_Add_ChildTransaction_Already_Exist()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var category = CreateCategory(SalaryCategoryName);
        var childTransaction1 = CreateTransaction(100, date, TransactionType.Inflow, category);
        var childTransaction2 = CreateTransaction(500, date, TransactionType.Outflow, category);

        // Act
        var compositeTransaction = CreateCompositeWithChildren(date, category, childTransaction1);

        // Assert
        Assert.Throws<InvalidOperationException>(() => compositeTransaction.AddChildTransaction(childTransaction1));
        Assert.Throws<InvalidOperationException>(() =>
            compositeTransaction.AddChildTransactions([childTransaction1, childTransaction2])
        );
    }

    [Fact]
    public void Should_Throw_Exception_When_Removing_ChildTransaction_That_Does_Not_Exist()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var category = CreateCategory(SalaryCategoryName);
        var childTransaction1 = CreateTransaction(100, date, TransactionType.Inflow, category);
        var childTransaction2 = CreateTransaction(50, date, TransactionType.Inflow, category);

        // Act
        var compositeTransaction = CreateCompositeWithChildren(date, category, childTransaction1);

        // Assert
        Assert.Throws<InvalidOperationException>(() => compositeTransaction.RemoveChildTransaction(childTransaction2));
    }

    [Fact]
    public void Should_Remove_ChildTransaction()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var category = CreateCategory(SalaryCategoryName);
        var childTransaction1 = CreateTransaction(100, date, TransactionType.Inflow, category);
        var childTransaction2 = CreateTransaction(50, date, TransactionType.Inflow, category);

        // Act
        var compositeTransaction = CreateCompositeWithChildren(date, category, childTransaction1, childTransaction2);
        compositeTransaction.RemoveChildTransaction(childTransaction1);

        // Assert
        Assert.Single(compositeTransaction.ChildTransactions);
        Assert.Equal(childTransaction2, compositeTransaction.ChildTransactions.First());
    }

    private static FinancialCategory CreateCategory(string name)
    {
        return new FinancialCategory { Name = name };
    }

    private static FinancialTransaction CreateTransaction(
        decimal value,
        DateOnly date,
        TransactionType type,
        FinancialCategory category
    )
    {
        return new FinancialTransactionDouble(value, date, type, category);
    }

    private static CompositeTransaction CreateCompositeTransaction(DateOnly date, FinancialCategory category)
    {
        return new CompositeTransaction
        {
            Date = date,
            Performed = false,
            Type = TransactionType.Inflow,
            Category = category,
            CategoryId = category.Id,
        };
    }

    private static CompositeTransaction CreateCompositeWithChildren(
        DateOnly date,
        FinancialCategory category,
        params FinancialTransaction[] childTransactions
    )
    {
        var composite = CreateCompositeTransaction(date, category);
        foreach (var transaction in childTransactions)
        {
            composite.AddChildTransaction(transaction);
        }

        return composite;
    }

    private sealed class FinancialTransactionDouble : FinancialTransaction
    {
        [SetsRequiredMembers]
        public FinancialTransactionDouble(
            decimal value,
            DateOnly date,
            TransactionType type,
            FinancialCategory category
        )
        {
            Date = date;
            Type = type;
            Category = category;
            CategoryId = category.Id;
            Performed = false;
            SetValue(value);
        }

        public override void SetValue(decimal value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Value cannot be negative.", nameof(value));
            }

            _value = value;
        }

        public override decimal GetValue()
        {
            return _value;
        }

        public override decimal GetValue(Expression<Func<FinancialTransaction, bool>> filter)
        {
            return filter.Compile()(this) ? _value : 0m;
        }
    }
}
