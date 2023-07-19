using DataPersistenceLayer.Entities;
using DataPersistenceLayer.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DataPersistenceLayer.Repositories;

public class HashRecordRepository : Repository<HashRecord>, IHashRecordRepository
{
    public HashRecordRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<IGrouping<DateTime, HashRecord>>> GetLatestRecordsAsGroupedByDateAsync(DateTime sinceDate)
    {
        return await _dbSet
            .Where(h => h.Date >= sinceDate)
            .GroupBy(h => h.Date.Date)
            .ToListAsync();
    }
}