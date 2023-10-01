using T3.Account.Api.Errors;

namespace T3.Account.Api.Settings;

public interface WebTokenSettings
{
    public string PrivateKeyPath { get; }
    public string PublicKeyPath { get; }
}

public class WebTokenSettingsProvider : WebTokenSettings
{
    public string PrivateKeyPath { get; }
    public string PublicKeyPath { get; }

    public WebTokenSettingsProvider(IConfiguration configuration)
    {
        PrivateKeyPath = configuration["WebTokenSettings:PrivateKeyPath"] ?? throw new ConfigurationException("Missing PrivateKeyPath");
        PublicKeyPath = configuration["WebTokenSettings:PublicKeyPath"] ?? throw new ConfigurationException("Missing PublicKeyPath");
    }
}