namespace T3.Account.Api.Errors;

public class WebTokenGeneratorException : Exception
{
    public WebTokenGeneratorException(string message) : base(message)
    {
    }
}