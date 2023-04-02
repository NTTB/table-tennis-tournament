using MongoDB.Driver;
using T3.Web.Services.Set.Models;

namespace T3.Web.Services.Set;

public record CreateSetRequest(string DisplayName);

public record CreateSetResponse(SetEntity Entity);

public interface ICreateSetService
{
    Task<CreateSetResponse> CreateSet(CreateSetRequest request);
}

public class CreateSetService : ICreateSetService
{
    private readonly IMongoCollection<SetEntity> _collection;

    public CreateSetService(IMongoCollection<SetEntity> collection)
    {
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
}