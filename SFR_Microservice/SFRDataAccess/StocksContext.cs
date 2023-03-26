using Demo;
using Microsoft.EntityFrameworkCore;
public class StocksContext:DbContext
{
    public DbSet<Bond> Bonds { get; set; }

    public StocksContext(DbContextOptions<StocksContext> options):base(options)
    {
        this.Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bond>()
            .HasNoKey();
        base.OnModelCreating(modelBuilder);
    }
}