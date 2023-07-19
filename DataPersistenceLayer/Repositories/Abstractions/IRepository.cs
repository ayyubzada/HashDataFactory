namespace DataPersistenceLayer.Repositories.Abstractions;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();
    Task<T> GetAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync();
}