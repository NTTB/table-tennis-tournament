using System.Diagnostics.CodeAnalysis;
using Redis.OM;
using Redis.OM.Searching;
using T3.Account.Api.Entities;
using T3.Account.Api.Repositories;
using T3.Account.Api.Services;
using T3.Account.Api.Settings;

namespace T3.Account.Api;

[ExcludeFromCodeCoverage(Justification = "Main entry point")]
internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Create redis connection
        builder.Services.AddScoped<RedisConnectionProvider>(provider =>
        {
            var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("Redis");
            return new RedisConnectionProvider(connectionString);
        });

        builder.Services.AddScoped<IRedisCollection<AccountEntity>>(provider =>
        {
            var redisProvider = provider.GetRequiredService<RedisConnectionProvider>();
            redisProvider.Connection.CreateIndex(typeof(AccountEntity));
            var redisCollection = redisProvider.RedisCollection<AccountEntity>();
            return redisCollection;
        });

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Repositories
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();

        // Services
        builder.Services.AddScoped<IAccountChangePasswordService, AccountChangePasswordService>();
        builder.Services.AddScoped<IAccountCreateService, AccountCreateService>();
        builder.Services.AddScoped<IAccountDeleteService, AccountDeleteService>();
        builder.Services.AddScoped<IAccountLoginService, AccountLoginService>();
        builder.Services.AddScoped<IAccountWebTokenGenerator, AccountWebTokenGenerator>();
        builder.Services.AddScoped<IPasswordService, PasswordV1Service>();
        builder.Services.AddSingleton<IWebTokenAlgorithmGenerator, WebTokenAlgorithmGenerator>();
        
        // Settings
        builder.Services.AddSingleton<AccountTokenSettings, AccountTokenSettingsProvider>();
        builder.Services.AddSingleton<PasswordV1Settings, PasswordV1SettingsProvider>();
        builder.Services.AddSingleton<WebTokenSettings, WebTokenSettingsProvider>();
        

        // Build the application
        var app = builder.Build();

        // Load the RSA keys from disk
        app.Services.GetRequiredService<IWebTokenAlgorithmGenerator>().LoadFromDisk().Wait();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

// Start the application
        app.Run();
    }
}