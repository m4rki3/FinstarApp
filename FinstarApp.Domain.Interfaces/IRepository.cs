namespace FinstarApp.Domain.Interfaces;

public interface IRepository<T> where T :class
{
    public Task<bool> TryAddAsync(T entity, CancellationToken token = default);
    public Task<T?> GetAsync(Guid id, CancellationToken token = default);
    public Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default);
    public Task<bool> TryUpdateAsync(T entity, CancellationToken token = default);
    public Task<bool> TryRemoveAsync(Guid id, CancellationToken token = default);
}