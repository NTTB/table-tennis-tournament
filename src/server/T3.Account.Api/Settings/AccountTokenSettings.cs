using T3.Account.Api.Errors;

namespace T3.Account.Api.Settings;

public interface AccountTokenSettings
{
    public string Issuer { get; }
}

public class AccountTokenSettingsProvider : AccountTokenSettings
{
    public string Issuer { get; }

    public AccountTokenSettingsProvider(IConfiguration configuration)
    {
        Issuer = configuration["AccountTokenSettings:Issuer"] ?? throw new ConfigurationException("Missing Issuer");
    }
}