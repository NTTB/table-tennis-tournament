namespace T3.Account.Api.Errors;

public class InvalidPasswordException : Exception
{
    public InvalidPasswordException(string message): base(message)
    {
    }
}