using MongoDB.Bson.Serialization;
using T3.Web.Services.Data;
using T3.Web.Services.SecretNotes.Entities;

namespace T3.Web.Services.SecretNotes;

public static class _SecretNoteModule
{
    public static IServiceCollection AddSecretNoteModule(this IServiceCollection collection)
    {
        var baseClassMap = BsonClassMap.RegisterClassMap<SecretNoteContent>();
        baseClassMap.AutoMap();
        
        foreach (var type in SecretNotePayloadTypes.GetTypes())
        {
            var explicitTypeMap = new BsonClassMap(type, baseClassMap);
            explicitTypeMap.AutoMap();
            BsonClassMap.RegisterClassMap(explicitTypeMap);
        }
        
        return collection
                .AddDbCollection<SecretNote>("secretNotes")
                .AddScoped<ISecretNoteService, SecretNoteService>()
            ;
    }
}