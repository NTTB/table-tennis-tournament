using T3.Data.Shared;

namespace T3.Web.Services.Match;

public static class MatchModule
{
    public static IServiceCollection AddMatchModule(this IServiceCollection collection)
    {
        return collection
                .AddDbCollection<MatchEntity>("matches")
                .AddScoped<IMatchService, MatchService>()
            ;
    }
}