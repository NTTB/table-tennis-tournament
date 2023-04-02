using MongoDB.Driver;
using T3.Web.Services.Set.Models;

namespace T3.Web.Services.Set;

public interface ISearchSetService
{
    Task<SetEntity> GetById(Guid setId);
    Task<SetEntity[]> GetAll();
}

public class SearchSetService : ISearchSetService
{
    private readonly IMongoCollection<SetEntity> _collection;

    public SearchSetService(IMongoCollection<SetEntity> collection)
    {
        _collection = collection;
    }

    public async Task<SetEntity> GetById(Guid setId)
    {
        return await _collection.Find(x => x.Id == setId).SingleOrDefaultAsync();
    }

    public async Task<SetEntity[]> GetAll()
    {
        return (await _collection.Find(FilterDefinition<SetEntity>.Empty).ToListAsync()).ToArray();
    }
}