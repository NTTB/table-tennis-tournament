using MongoDB.Bson.Serialization;
using T3.Web.Services.Commit.Models;
using T3.Web.Services.Data;

namespace T3.Web.Services.Commit;

public static class CommitModule
{
    public static IServiceCollection AddCommitModule(this IServiceCollection collection)
    {
        BsonClassMap.RegisterClassMap<SetCommitCommand>();
        BsonClassMap.RegisterClassMap<NoOpCommand>();
        BsonClassMap.RegisterClassMap<SetAwayPlayersCommand>();
        BsonClassMap.RegisterClassMap<SetHomePlayersCommand>();
        BsonClassMap.RegisterClassMap<SetInitialServiceCommand>();
        BsonClassMap.RegisterClassMap<SetCurrentServiceCommand>();
        BsonClassMap.RegisterClassMap<ChangeSetScoreCommand>();

        return collection
            .AddDbCollection<SetCommit>("setCommits")
            .AddScoped<ISetCommitService, SetCommitService>()
            ;
    }
}