using MongoDB.Driver;
using T3.Web.Services.Set.Models;

namespace T3.Web.Services.Set;

public interface ISetCommitService
{
    Task<IEnumerable<SetCommit>> GetAll(SetId setId);
    Task Add(SetCommit commit);
}

public class SetCommitService : ISetCommitService
{
    private readonly ILogger<SetCommitService> _logger;
    private readonly IMongoCollection<SetCommit> _collection;

    public SetCommitService(
        ILogger<SetCommitService> logger,
        IMongoCollection<SetCommit> collection)
    {
        _logger = logger;
        _collection = collection;
    }

    public async Task<IEnumerable<SetCommit>> GetAll(SetId setId)
    {
        return await _collection.Find(x => x.Header.SetId.Value == setId.Value).ToListAsync();
    }

    public async Task Add(SetCommit commit)
    {
        await _collection.InsertOneAsync(commit);
    }
}