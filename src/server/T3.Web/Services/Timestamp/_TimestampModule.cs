using T3.Data.Shared;

namespace T3.Web.Services.Timestamp;

public static class TimestampModule
{
    public static IServiceCollection AddTimestampModule(this IServiceCollection collection)
    {
        return collection
            .AddDbCollection<ServerTimestampEntity>("serverTimestamps")
            .AddScoped<ITimestampService, TimestampService>()
            ;
    }
}