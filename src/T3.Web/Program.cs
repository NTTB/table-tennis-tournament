using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using T3.Web.Services.Identity;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var connectionStringData = config.GetConnectionString("ttt-data");
var connectionStringIdentity = config.GetConnectionString("identity");

// builder.Configuration.
// Add services to the container

builder.Services.AddIdentityServices(config);

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
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers();


var app = builder.Build();


app.UseDeveloperExceptionPage();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();