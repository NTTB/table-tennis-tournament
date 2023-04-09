namespace T3.Web.Services.Shared;

public static class SharedModule
{
    public static IServiceCollection AddSharedModule(this IServiceCollection collection)
    {
        return collection
            .AddScoped<IDateTimeService, DateTimeService>();
    }
}