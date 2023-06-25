using T3.Web.Services.Data;
using T3.Web.Services.Set.Entities;

namespace T3.Web.Services.Set;

public static class SetModule
{
    public static IServiceCollection AddSetModule(this IServiceCollection collection)
    {
        return collection
                .AddDbCollection<SetEntity>("sets")
                .AddScoped<ISetService, SetService>()
            ;
    }
}