using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using T3.Data.Shared;
using T3.Web.Hubs;
using T3.Web.Services.Commit;
using T3.Web.Services.Identity;
using T3.Web.Services.Match;
using T3.Web.Services.MatchValidation;
using T3.Web.Services.Shared;
using T3.Web.Services.Timestamp;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container

var jsonConverters = new JsonConverter[]
{
    new JsonStringEnumConverter(),
    new MatchCommitBodyConvertor()
};

builder.Services
    .AddSharedModule()
    .AddDataService(sp => sp.GetRequiredService<IConfiguration>().GetConnectionString("Data") ?? throw new Exception("No connection string found"))
    .AddIdentityServices(config)
    .AddMatchModule()
    .AddSetValidationModule()
    .AddTimestampModule()
    .AddCommitModule()
    .AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
    }).AddJsonProtocol(options =>
    {
        foreach (var converter in jsonConverters)
        {
            options.PayloadSerializerOptions.Converters.Add(converter);
        }
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var settings = config.GetSection(nameof(AccountTokenService)).Get<AccountTokenService.Settings>()
                       ?? throw new Exception("Could not find settings for AccountTokenService");
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidAudience = settings.Audience,
            ValidIssuer = settings.Issuer,
            LogValidationExceptions = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    foreach (var converter in jsonConverters)
    {
        options.JsonSerializerOptions.Converters.Add(converter);
    }
});


var app = builder.Build();

// Run the database migrations.
var migrationRunner =app.Services.GetRequiredService<IMigrationService>();
migrationRunner.PerformMigration();

app.UseDeveloperExceptionPage();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller}/{action=Index}/{id?}");

app.MapHub<MatchHub>("/hubs/set");

app.MapFallbackToFile("index.html");

app.Run();