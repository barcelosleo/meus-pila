using MeusPila.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeusPila.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<FinancialCategory> FinancialCategories => Set<FinancialCategory>();
    public DbSet<GroupUser> GroupUsers => Set<GroupUser>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<FinancialTransaction> FinancialTransactions => Set<FinancialTransaction>();
    public DbSet<IndividualTransaction> IndividualTransactions => Set<IndividualTransaction>();
    public DbSet<CompositeTransaction> CompositeTransactions => Set<CompositeTransaction>();
    public DbSet<InstallmentTransaction> InstallmentTransactions => Set<InstallmentTransaction>();
    public DbSet<AccountTransaction> FinancialAccountTransactions => Set<AccountTransaction>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Setting the discriminator for FinancialAccount and its derived types
        modelBuilder
            .Entity<FinancialAccount>()
            .HasDiscriminator<string>("AccountType")
            .HasValue<BankAccount>("BankAccount")
            .HasValue<CreditCard>("CreditCard");

        // Setting the composite structure for FinancialTransaction and its derived types
        modelBuilder
            .Entity<FinancialTransaction>()
            .HasDiscriminator<string>("TransactionKind")
            .HasValue<IndividualTransaction>("Individual")
            .HasValue<CompositeTransaction>("Composite")
            .HasValue<InstallmentTransaction>("Installment");

        // Setting the relationship between FinancialTransaction and Accounts (FinancialAccount and CreditCard) through AccountTransaction
        modelBuilder
            .Entity<AccountTransaction>()
            .HasKey(bat => new { bat.FinancialAccountId, bat.FinancialTransactionId });

        // Setting the relationship between User and Household through HouseholdUser
        modelBuilder.Entity<GroupUser>().HasKey(hu => new { hu.UserId, hu.GroupId });
    }
}
