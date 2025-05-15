using FinstarApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinstarApp.Infrastructure.Data;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;
    private readonly ILogger<Repository<T>> _logger;
    
    public Repository(DbContext dbContext, ILogger<Repository<T>> logger)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
        _logger = logger;
    }
    
    public async Task<bool> TryAddAsync(T entity, CancellationToken token = default)
    {
        await _dbSet.AddAsync(entity, token);
        try
        {
            await _dbContext.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<T?> GetAsync(Guid id, CancellationToken token = default)
    {
        return await _dbSet.FindAsync([id], token);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(token);
    }

    public async Task<bool> TryUpdateAsync(T entity, CancellationToken token = default)
    {
        _dbSet.Update(entity);
        try
        {
            await _dbContext.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<bool> TryRemoveAsync(Guid id, CancellationToken token = default)
    {
        var entity = await _dbSet.FindAsync([id], token);
        if (entity is null)
        {
            _logger.LogWarning("Task is not found");
            return false;
        }
        
        _dbSet.Remove(entity);
        try
        {
            await _dbContext.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }
}