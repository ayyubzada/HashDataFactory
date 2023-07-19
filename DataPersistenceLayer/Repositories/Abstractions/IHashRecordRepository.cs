using DataPersistenceLayer.Entities;

namespace DataPersistenceLayer.Repositories.Abstractions;

public interface IHashRecordRepository : IRepository<HashRecord>
{
    Task<IEnumerable<IGrouping<DateTime, HashRecord>>> GetLatestRecordsAsGroupedByDateAsync(DateTime sinceDate);
}