﻿namespace T3.Account.Api.Errors;

public class LoginException : Exception
{
    public LoginException(string message) : base(message)
    {
    }
}