using MongoDB.Driver;
using TTT.Web.Services.Identity;
using TTT.Web.Services.Identity.Models;

public static class ModuleIdentity
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection collection,
        ConfigurationManager config
    )
    {
        // Add the settings
        collection
            .Configure<PasswordV1Service.Settings>(config.GetSection(nameof(PasswordV1Service)))
            .Configure<AccountTokenService.Settings>(config.GetSection(nameof(AccountTokenService)));

        // Register two IMongoClient instances that are connected to different databases ("identity" and "data")
        collection
            .AddScoped<IMongoClient>(_ => new MongoClient(config.GetConnectionString("data")))
            .AddScoped<IMongoDatabase>(sc => sc.GetRequiredService<IMongoClient>().GetDatabase("ttt-data"))
            .AddScoped<IMongoCollection<AccountEntity>>(sc =>
                sc.GetRequiredService<IMongoDatabase>().GetCollection<AccountEntity>("accounts"));

        // Add the services
        return collection
                .AddScoped<IPasswordService, PasswordV1Service>()
                .AddScoped<IAccountCreateService, AccountCreateService>()
                .AddScoped<IAccountLoginService, AccountLoginService>()
                .AddScoped<IAccountTokenService, AccountTokenService>()
            ;
    }
}