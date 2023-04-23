using MongoDB.Bson.Serialization;
using T3.Web.Services.Commit.Models;
using T3.Web.Services.Data;

namespace T3.Web.Services.Commit;

public static class CommitModule
{
    public static IServiceCollection AddCommitModule(this IServiceCollection collection)
    {
        var baseClassMap = BsonClassMap.RegisterClassMap<SetCommitCommand>();
        baseClassMap.AutoMap();
        
        foreach (var type in SetCommitBodyTypes.GetTypes())
        {
            var explicitTypeMap = new BsonClassMap(type, baseClassMap);
            explicitTypeMap.AutoMap();
            BsonClassMap.RegisterClassMap(explicitTypeMap);
        }

        return collection
            .AddDbCollection<SetCommit>("setCommits")
            .AddScoped<ISetCommitService, SetCommitService>()
            ;
    }
}