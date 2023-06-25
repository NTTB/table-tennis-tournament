using MongoDB.Driver;
using T3.Web.Services.Data.Entities;
using T3.Web.Services.Data.Migrations;

namespace T3.Web.Services.Data;

public static class ModuleData
{
    public static IServiceCollection AddDataService(
        this IServiceCollection collection,
        ConfigurationManager config
    )
    {
        // Add the settings
        return collection
                .AddSingleton<MongoUrl>(_ => new MongoUrl(config.GetConnectionString("data")))
                .AddSingleton<IMongoClient>(sc => new MongoClient(sc.GetRequiredService<MongoUrl>()))
                .AddSingleton<IMongoDatabase>(sc =>
                {
                    var client = sc.GetRequiredService<IMongoClient>();
                    var connectionString = sc.GetRequiredService<MongoUrl>();
                    return client.GetDatabase(connectionString.DatabaseName);
                })
                .AddDbCollection<MigrationEntity>("_migrations")
                .AddTransient<IMigrationService, MigrationService>()
                .AddMigrations()
            ;
    }

    private static IServiceCollection AddMigrations(this IServiceCollection collection)
    {
        // Get all migrations by reflection
        var migrations = typeof(ModuleData).Assembly.GetTypes()
            .Where(x => typeof(IMigration).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToList();

        // Register all migrations
        foreach (var migration in migrations) collection.AddTransient(typeof(IMigration), migration);


        return collection;
    }

    public static IServiceCollection AddDbCollection<T>(this IServiceCollection collection, string collectionName)
    {
        return collection.AddTransient<IMongoCollection<T>>(sp =>
            sp.GetRequiredService<IMongoDatabase>().GetCollection<T>(collectionName));
    }
}