using Redis.OM;
using Redis.OM.Searching;
using StackExchange.Redis;
using T3.Account.Api.Entities;
using T3.Account.Api.Repositories;
using T3.Account.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Create redis connection

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
RedisConnectionProvider redisConnectionProvider = new(redis);
// Create redis collections
var accountCollection = redisConnectionProvider.RedisCollection<AccountEntity>();

builder.Services.AddScoped<RedisConnectionProvider>(provider =>
{
    return new RedisConnectionProvider(provider.GetRequiredService<IConfiguration>().GetConnectionString("Redis"));
});

builder.Services.AddScoped<IRedisCollection<AccountEntity>>(provider =>
{
    return provider.GetRequiredService<RedisConnectionProvider>().RedisCollection<AccountEntity>();
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Services
builder.Services.AddScoped<IPasswordService, PasswordV1Service>();
builder.Services.AddScoped<IAccountCreateService, AccountCreateService>();
builder.Services.AddScoped<IAccountChangePasswordService, AccountChangePasswordService>();
builder.Services.AddScoped<IAccountDeleteService, AccountDeleteService>();


// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Start the application
app.Run();