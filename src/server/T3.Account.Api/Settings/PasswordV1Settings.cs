using T3.Account.Api.Errors;

namespace T3.Account.Api.Settings;

public class PasswordV1Settings
{
    public int SaltSize { get; set; }
    public int HashSize { get; set; }
    public int Iterations { get; set; }
}

public class PasswordV1SettingsProvider : PasswordV1Settings
{
    public PasswordV1SettingsProvider(IConfiguration configuration)
    {
        SaltSize = int.Parse(configuration["PasswordV1Settings:SaltSize"] ?? throw new ConfigurationException("Missing SaltSize"));
        HashSize = int.Parse(configuration["PasswordV1Settings:HashSize"] ?? throw new ConfigurationException("Missing HashSize"));
        Iterations = int.Parse(configuration["PasswordV1Settings:Iterations"] ?? throw new ConfigurationException("Missing Iterations"));
    }
}