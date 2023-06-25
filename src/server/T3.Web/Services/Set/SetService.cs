using MongoDB.Driver;
using T3.Web.Services.Set.Entities;

namespace T3.Web.Services.Set;

public record CreateSetRequest(string DisplayName);

public record CreateSetResponse(SetEntity Entity);

public interface ISetService
{
    Task<SetEntity> GetById(Guid setId);
    Task<SetEntity[]> GetAll();
    Task DeleteById(Guid setId);

    Task<CreateSetResponse> CreateSet(CreateSetRequest request);
}

public class SetService : ISetService
{
    private readonly IMongoCollection<SetEntity> _collection;
    private readonly ILogger<SetService> _logger;

    public SetService(
        ILogger<SetService> logger,
        IMongoCollection<SetEntity> collection)
    {
        _logger = logger;
        _collection = collection;
    }

    public async Task<CreateSetResponse> CreateSet(CreateSetRequest request)
    {
        var entity = new SetEntity
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName
        };

        return await _collection
            .InsertOneAsync(entity)
            .ContinueWith(_ => new CreateSetResponse(entity));
    }

    public async Task<SetEntity> GetById(Guid setId)
    {
        return await _collection.Find(x => x.Id == setId).SingleOrDefaultAsync();
    }

    public async Task<SetEntity[]> GetAll()
    {
        return (await _collection.Find(FilterDefinition<SetEntity>.Empty).ToListAsync()).ToArray();
    }

    public async Task DeleteById(Guid setId)
    {
        _logger.LogInformation("Deleting set with id {SetId}", setId);
        await _collection.DeleteOneAsync(x => x.Id == setId);
    }
}