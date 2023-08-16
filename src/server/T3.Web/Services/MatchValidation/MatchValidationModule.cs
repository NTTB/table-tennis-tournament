namespace T3.Web.Services.MatchValidation;

public static class MatchValidationModule
{
    public static IServiceCollection AddSetValidationModule(this IServiceCollection collection)
    {
        return collection
                .AddScoped<IMatchValidateService, MatchValidateService>()
            ;
    }
}