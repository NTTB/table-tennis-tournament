using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using TTT.Web.Policy;
using TTT.Web.Services.Identity;
using TTT.Web.Services.Mailing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IMongoClient>((provider) =>
{
    var client = new MongoClient(builder.Configuration.GetConnectionString("ttt-data"));
    return client;
});

builder.Services.AddIdentityMongoDbProvider<ApplicationUser>
(
    identity => { },
    mongo => { mongo.ConnectionString = builder.Configuration.GetConnectionString("identity"); }
);

builder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, HasClaimHandler>();

builder.Services.AddSingleton<IEmailSender, EmailSender>();

// builder.Services.AddIdentityServer()
//     .AddApiAuthorization<>()
// builder.Services.AddAuthentication().AddIdentityServerJwt();

builder.Services.AddControllersWithViews();

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