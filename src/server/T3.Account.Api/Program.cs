using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
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
            // TODO: How do we handle key rotation without restarting the service?
            var publicKey = RSA.Create(2048);
            publicKey.ImportFromPem(File.ReadAllText(webTokenSettings.PublicKeyPath));
            
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                // The issuer is always verified since there can only be one issuer (this http-api)
                ValidateIssuer = true,
                ValidIssuer = tokenSettings.Issuer,
                
                // TODO: Do we need to verify the audience? We only need to verify that we are the issuer.
                ValidateAudience = true,
                ValidAudience = "string",
                
                // TODO: How do we verify signing keys that are expired? Can we provide the user with a smooth transition?
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(publicKey),
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    // Find the 'sub' claim and set it as the 'Name' property.
                    if (context.Principal == null) return Task.CompletedTask;
                    var subClaim = context.Principal.FindFirst("sub");
                    if (subClaim == null) return Task.CompletedTask;
                    if (context.Principal.Identity is not ClaimsIdentity identity) return Task.CompletedTask;
                    identity.AddClaim(new Claim(ClaimTypes.Name, subClaim.Value));
                    return Task.CompletedTask;
                }
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
                    Scheme = "oauth2",
                    Reference = new OpenApiReference()
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme,
                    },
                    In = ParameterLocation.Header,
                    Name = "Bearer"
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