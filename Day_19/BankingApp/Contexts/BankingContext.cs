using Microsoft.EntityFrameworkCore;

public class BankingContext : DbContext
{
    public BankingContext(DbContextOptions<BankingContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transactions> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(e =>
        {
            e.HasKey(a => a.AccountId);
            e.Property(a => a.AccountHolder)
                .IsRequired()
                .HasMaxLength(100);
            e.Property(a => a.AccountType)
                .IsRequired()
                .HasMaxLength(50);
            e.Property(a => a.Balance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);
        });

        modelBuilder.Entity<Transactions>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            e.Property(t => t.TransactionType)
                .IsRequired()
                .HasMaxLength(50);
            e.Property(t => t.TransactionDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            e.HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}