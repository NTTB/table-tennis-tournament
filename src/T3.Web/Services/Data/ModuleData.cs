using MongoDB.Driver;

namespace T3.Web.Services.Data;

public static class DataIdentity
{
    public static IServiceCollection AddDataService(
        this IServiceCollection collection,
        ConfigurationManager config
    )
    {
        // Add the settings
        return collection
            .AddScoped<MongoUrl>(_ => new MongoUrl(config.GetConnectionString("data")))
            .AddScoped<IMongoClient>(sc => new MongoClient(sc.GetRequiredService<MongoUrl>()))
            .AddScoped<IMongoDatabase>(sc =>
            {
                var client = sc.GetRequiredService<IMongoClient>();
                var connectionString = sc.GetRequiredService<MongoUrl>();
                return client.GetDatabase(connectionString.DatabaseName);
            });
    }
    
    public static IServiceCollection AddDbCollection<T>(this IServiceCollection collection, string collectionName)
    {
        return collection.AddScoped<IMongoCollection<T>>(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<T>(collectionName));
    }
}