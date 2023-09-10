namespace T3.Account.Api.Errors;

public class CreateAccountException : Exception
{
    public CreateAccountException(string message) : base(message)
    {
    }
}