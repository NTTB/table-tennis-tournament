namespace T3.Account.Api.Errors;

public class DeleteAccountException : Exception
{
    public DeleteAccountException(string message) : base(message)
    {
    }
}