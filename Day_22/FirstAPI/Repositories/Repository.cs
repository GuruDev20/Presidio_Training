using FirstAPI.Contexts;

public abstract class Repository<K, T> : IRepository<K, T> where T : class
{
    protected readonly ClinicContext _clinicContext;
    public Repository(ClinicContext clinicContext)
    {
        _clinicContext = clinicContext;
    }
    public async Task<T> Add(T item)
    {
        try
        {
            _clinicContext.Add(item);
            await _clinicContext.SaveChangesAsync();
            return item;
        }
        catch (Exception e)
        {
            throw new Exception("Error adding item", e);
        }
    }
    public async Task<T> Delete(K key)
    {
        var item = await Get(key);
        if (item != null)
        {
            _clinicContext.Remove(item);
            await _clinicContext.SaveChangesAsync();
            return item;
        }
        throw new KeyNotFoundException("Item not found");
    }
    public abstract Task<T> Get(K key);
    public abstract Task<IEnumerable<T>> GetAll();
    public async Task<T> Update(K key,T item)
    {
        var existingItem = await Get(key);
        if (existingItem != null)
        {
            _clinicContext.Entry(existingItem).CurrentValues.SetValues(item);
            await _clinicContext.SaveChangesAsync();
            return item;
        }
        throw new KeyNotFoundException("Item not found");
    }
    
}
