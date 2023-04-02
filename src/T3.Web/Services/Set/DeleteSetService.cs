using MongoDB.Driver;
using T3.Web.Services.Set.Models;

namespace T3.Web.Services.Set;

public interface IDeleteSetService
{
    Task DeleteById(Guid setId);
}

public class DeleteSetService : IDeleteSetService
{
    private readonly ILogger<DeleteSetService> _logger;
    private readonly IMongoCollection<SetEntity> _collection;

    public DeleteSetService(
        ILogger<DeleteSetService> logger,
        IMongoCollection<SetEntity> collection)
    {
        _logger = logger;
        _collection = collection;
    }

    public async Task DeleteById(Guid setId)
    {
        _logger.LogInformation("Deleting set with id {SetId}", setId);
        await _collection.DeleteOneAsync(x => x.Id == setId);
    }
}