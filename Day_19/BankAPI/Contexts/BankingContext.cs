using Microsoft.EntityFrameworkCore;

public class BankingContext : DbContext
{
    public BankingContext(DbContextOptions<BankingContext> options) : base(options) { }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transactions> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasKey(a => a.AccountNumber);

        modelBuilder.Entity<Transactions>()
            .HasKey(t => t.TransactionId);

        modelBuilder.Entity<Account>()
            .HasMany(a => a.Transactions)
            .WithOne(t => t.Account!)
            .HasForeignKey(t => t.AccountNumber)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Account>()
            .Property(a => a.AccountNumber)
            .IsRequired()
            .HasMaxLength(20);

        modelBuilder.Entity<Transactions>()
            .Property(t => t.TransactionType)
            .IsRequired()
            .HasMaxLength(20);

    }
}