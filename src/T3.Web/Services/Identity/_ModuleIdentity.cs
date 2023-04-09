using MongoDB.Driver;
using T3.Web.Services.Identity.Entities;

namespace T3.Web.Services.Identity;

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

        collection.AddDbCollection<AccountEntity>("accounts")
            ;

        // Add the services
        return collection
                .AddScoped<IPasswordService, PasswordV1Service>()
                .AddScoped<IAccountCreateService, AccountCreateService>()
                .AddScoped<IAccountLoginService, AccountLoginService>()
                .AddScoped<IAccountTokenService, AccountTokenService>()
            ;
    }
    
    private static IServiceCollection AddDbCollection<T>(this IServiceCollection collection, string collectionName)
    {
        return collection.AddScoped<IMongoCollection<T>>(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<T>(collectionName));
    }
}