using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using T3.Web.Hubs;
using T3.Web.Services.Data;
using T3.Web.Services.Identity;
using T3.Web.Services.Set;
using T3.Web.Services.Shared;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container

builder.Services
    .AddSharedModule()
    .AddDataService(config)
    .AddIdentityServices(config)
    .AddSetModule()
    .AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
    }).AddJsonProtocol(x=>
    {
        x.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        x.PayloadSerializerOptions.Converters.Add(new SetCommitBodyConvertor());
    })
    ;

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
builder.Services.AddControllers();


var app = builder.Build();


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

app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<SetHub>("/hubs/set");

app.MapFallbackToFile("index.html");

app.Run();