namespace DataPersistenceLayer.Repositories.Abstractions;

public interface IUnitOfWork
{
    T GetRepository<T>() where T : class;
    Task<int> SaveChangesAsync();
}