using MongoDB.Bson.Serialization;
using T3.Web.Services.Commit.Models;
using T3.Web.Services.Data;

namespace T3.Web.Services.Commit;

public static class CommitModule
{
    public static IServiceCollection AddCommitModule(this IServiceCollection collection)
    {
        BsonClassMap.RegisterClassMap<SetCommitBody>();
        BsonClassMap.RegisterClassMap<SetCommitBodyNoOp>();
        BsonClassMap.RegisterClassMap<SetCommitBodySetScoreChange>();

        return collection
            .AddDbCollection<SetCommit>("setCommits")
            .AddScoped<ISetCommitService, SetCommitService>()
            ;
    }
}