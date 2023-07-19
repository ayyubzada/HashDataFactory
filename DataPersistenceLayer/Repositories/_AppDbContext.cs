using DataPersistenceLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataPersistenceLayer.Repositories;

public class AppDbContext : DbContext
{
    public DbSet<HashRecord> HashRecords { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        if (Database.GetPendingMigrations().Any())
        {
            Database.Migrate();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HashRecord>()
            .Property(h => h.Sha1)
            .IsRequired()
            .HasMaxLength(150);

        base.OnModelCreating(modelBuilder);
    }
}