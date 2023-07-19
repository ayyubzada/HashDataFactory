using DataPersistenceLayer.Repositories.Abstractions;

namespace DataPersistenceLayer.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private Dictionary<Type, object> _repositories;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    public T GetRepository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            switch (typeof(T))
            {
                case IHashRecordRepository:
                    _repositories[type] = new HashRecordRepository(_context);
                    break;
                default:
                    _repositories[type] = new Repository<T>(_context);
                    break;
            }
        }

        return (T)_repositories[type];
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}