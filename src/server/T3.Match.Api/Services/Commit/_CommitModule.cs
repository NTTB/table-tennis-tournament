using MongoDB.Bson.Serialization;
using T3.Data.Shared;

namespace T3.Web.Services.Commit;

public static class CommitModule
{
    public static IServiceCollection AddCommitModule(this IServiceCollection collection)
    {
        var baseClassMap = BsonClassMap.RegisterClassMap<MatchCommitCommand>();
        baseClassMap.AutoMap();
        
        foreach (var type in MatchCommitBodyTypes.TypeMap.Values)
        {
            var explicitTypeMap = new BsonClassMap(type, baseClassMap);
            explicitTypeMap.AutoMap();
            BsonClassMap.RegisterClassMap(explicitTypeMap);
        }

        return collection
            .AddDbCollection<MatchCommitEntity>("setCommits")
            .AddScoped<IMatchCommitService, MatchCommitService>()
            ;
    }
}