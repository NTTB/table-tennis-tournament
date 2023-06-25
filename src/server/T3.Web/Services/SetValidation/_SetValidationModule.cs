namespace T3.Web.Services.SetValidation;

public static class _SetValidationModule
{
    public static IServiceCollection AddSetValidationModule(this IServiceCollection collection)
    {
        return collection
                .AddScoped<ISetCommitValidateService, SetCommitValidateService>()
            ;
    }
}