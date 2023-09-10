using MongoDB.Driver;

namespace T3.Web.Services.Match;

public record CreateMatchRequest(string DisplayName);

public record CreateMatchResponse(MatchEntity Entity);

public interface IMatchService
{
    Task<MatchEntity> GetById(Guid setId);
    Task<MatchEntity[]> GetAll();
    Task DeleteById(Guid setId);

    Task<CreateMatchResponse> Create(CreateMatchRequest request);
}

public class MatchService : IMatchService
{
    private readonly IMongoCollection<MatchEntity> _collection;
    private readonly ILogger<MatchService> _logger;

    public MatchService(
        ILogger<MatchService> logger,
        IMongoCollection<MatchEntity> collection)
    {
        _logger = logger;
        _collection = collection;
    }

    public async Task<CreateMatchResponse> Create(CreateMatchRequest request)
    {
        var entity = new MatchEntity
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName
        };

        return await _collection
            .InsertOneAsync(entity)
            .ContinueWith(_ => new CreateMatchResponse(entity));
    }

    public async Task<MatchEntity> GetById(Guid setId)
    {
        return await _collection.Find(x => x.Id == setId).SingleOrDefaultAsync();
    }

    public async Task<MatchEntity[]> GetAll()
    {
        return (await _collection.Find(FilterDefinition<MatchEntity>.Empty).ToListAsync()).ToArray();
    }

    public async Task DeleteById(Guid setId)
    {
        _logger.LogInformation("Deleting set with id {SetId}", setId);
        await _collection.DeleteOneAsync(x => x.Id == setId);
    }
}