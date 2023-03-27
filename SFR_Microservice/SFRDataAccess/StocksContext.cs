using Demo;
using Microsoft.EntityFrameworkCore;
public class StocksContext:DbContext
{
    public DbSet<BondEntity> Bonds { get; set; }
    public DbSet<BondTrendEntity> BondTrends { get; set; }
    public StocksContext(DbContextOptions<StocksContext> options):base(options)
    {
        this.Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}