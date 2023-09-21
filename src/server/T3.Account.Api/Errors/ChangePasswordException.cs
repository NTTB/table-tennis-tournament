namespace T3.Account.Api.Errors;

public class ChangePasswordException : Exception
{
    public ChangePasswordException(string message): base(message)
    {
    }
}