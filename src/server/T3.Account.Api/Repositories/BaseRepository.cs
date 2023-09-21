using Redis.OM.Searching;

namespace T3.Account.Api.Repositories;

public abstract class BaseRepository<T> : IRepository<T>
{
    protected readonly IRedisCollection<T> Collection;

    protected BaseRepository(IRedisCollection<T> collection)
    {
        Collection = collection;
    }

    public virtual async Task InsertOne(T entity)
    {
        await Collection.InsertAsync(entity);
    }

    public virtual async Task UpdateOne(T entity)
    {
        await Collection.UpdateAsync(entity);
    }

    public async Task DeleteOne(T entity)
    {
        await Collection.DeleteAsync(entity);
    }
}