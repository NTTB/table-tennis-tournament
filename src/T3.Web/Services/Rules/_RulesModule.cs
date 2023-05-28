using T3.Web.Services.Data;
using T3.Web.Services.Rules.Entities;

namespace T3.Web.Services.Rules;

public static class RulesModule
{
    public static IServiceCollection AddRulesModule(this IServiceCollection collection)
    {
        return collection
                .AddDbCollection<RuleBookEntity>("rulebooks")
                .AddScoped<IRulebookService, RulebookService>()
            ;
    }
}