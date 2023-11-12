using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Redis.OM;
using Redis.OM.Searching;
using T3.Account.Api.Entities;
using T3.Account.Api.Errors;
using T3.Account.Api.Repositories;
using T3.Account.Api.Services;
using T3.Account.Api.Settings;

namespace T3.Account.Api;

// ReSharper disable once ClassNeverInstantiated.Global since this is the entry point
[ExcludeFromCodeCoverage(Justification = "Main entry point")]
internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Create redis connection
        builder.Services.AddScoped<RedisConnectionProvider>(provider =>
        {
            var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("Redis") ??
                                   throw new ConfigurationException("Missing Redis connection string");
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

        builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var tokenSettings = new AccountTokenSettingsProvider(builder.Configuration);
            var webTokenSettings = new WebTokenSettingsProvider(builder.Configuration);
            
            // NOTE: The keys are always generated on startup and loaded from disk.
            // And this function is only called when we need to validate a token so the files should exist at this point.
            var publicKey = RSA.Create(2048);
            publicKey.ImportFromPem(File.ReadAllText(webTokenSettings.PublicKeyPath));
            
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                // The issuer is always verified since there can only be one issuer (this http-api)
                ValidateIssuer = true,
                ValidIssuer = tokenSettings.Issuer,
                
                // The audience is always verified since there can only be one audience when interacting with this http-api
                ValidateAudience = true,
                ValidAudience = tokenSettings.Issuer,
                
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(publicKey),
                
            };
        });

        builder.Services.AddAuthorization();

        // Build the application
        var app = builder.Build();

        // Load the RSA keys from disk
        app.Services.GetRequiredService<IWebTokenAlgorithmGenerator>().LoadFromDisk().Wait();

        // Configure the HTTP request pipeline.
        app.UseSwagger(options =>
        {
            options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                var openApiSecurityRequirement = new OpenApiSecurityRequirement();
                var securityScheme = new OpenApiSecurityScheme
                {
                    Description =  "JWT Authorization header using the Bearer scheme.\n\n",
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http,
                    Reference = new OpenApiReference()
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    },
                    In = ParameterLocation.Header,
                    Name = "Bearer",
                    
                };

                openApiSecurityRequirement.Add(securityScheme, new List<string>());
                swaggerDoc.SecurityRequirements.Add(openApiSecurityRequirement);
                swaggerDoc.Components.SecuritySchemes.Add("Bearer", securityScheme);
            });
        });
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Start the application
        app.Run();
    }
}