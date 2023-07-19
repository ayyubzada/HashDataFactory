using DataPersistenceLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataPersistenceLayer.Repositories;

public class AppDbContext : DbContext
{
    public DbSet<HashRecord> HashRecords { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}