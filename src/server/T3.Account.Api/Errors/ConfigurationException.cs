namespace T3.Account.Api.Errors;

public class ConfigurationException : Exception
{
    public ConfigurationException(string message) : base(message)
    {
    }
}