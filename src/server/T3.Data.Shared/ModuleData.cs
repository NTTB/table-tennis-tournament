using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace T3.Data.Shared;

public static class ModuleData
{
    public static IServiceCollection AddDataService(
        this IServiceCollection collection,
        Func<IServiceProvider, string> connectionStringProvider,
        string migrationCollectionName = "_migrations")
    {
        // Add the settings
        return collection
                .AddSingleton<MongoUrl>(sp => new MongoUrl(connectionStringProvider(sp)))
                .AddSingleton<IMongoClient>(sc => new MongoClient(sc.GetRequiredService<MongoUrl>()))
                .AddSingleton<IMongoDatabase>(sp =>
                {
                    var client = sp.GetRequiredService<IMongoClient>();
                    var connectionString = sp.GetRequiredService<MongoUrl>();
                    return client.GetDatabase(connectionString.DatabaseName);
                })
                .AddDbCollection<MigrationEntity>(migrationCollectionName)
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