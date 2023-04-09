using MongoDB.Bson.Serialization;
using T3.Web.Services.Data;
using T3.Web.Services.Set.Entities;
using T3.Web.Services.Set.Models;

namespace T3.Web.Services.Set;

public static class SetModule
{
    public static IServiceCollection AddSetModule(this IServiceCollection collection)
    {
        BsonClassMap.RegisterClassMap<ISetCommitBody>();

        return collection
            .AddDbCollection<SetEntity>("sets")
            .AddDbCollection<ServerTimestampEntity>("serverTimestamps")
            .AddDbCollection<SetCommit>("setCommits")
            .AddScoped<ITimestampService, TimestampService>()
            .AddScoped<ISetService, SetService>()
            .AddScoped<ISetCommitService, SetCommitService>()
            ;
        
        
    }
}