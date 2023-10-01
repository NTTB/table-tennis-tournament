using System.Security.Cryptography;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework.Internal;
using T3.Account.Api.Values;

namespace T3.Account.Api.Test;

/// <summary>
/// This class generate random data for testing.
/// </summary>
public static class RandomData
{
    public static string NextUsername()
    {
        var length = Randomizer.Next(3, 20);
        return Randomizer.GetString(length, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
    }

    public static string NextPassword()
    {
        var length = Randomizer.Next(12, 32);
        return Randomizer.GetString(length);
    }

    public static PasswordValue NextPasswordValue()
    {
        return new PasswordValue()
        {
            Hash = NextByteArray(10),
            Salt = NextByteArray(10),
            Iterations = NextNumber(),
            Version = "fake",
        };
    }

    private static Randomizer Randomizer => CurrentContext.Random;
    private static TestContext CurrentContext => TestContext.CurrentContext;

    public static byte[] NextByteArray(int length)
    {
        var result = new byte[length];
        Randomizer.NextBytes(result);
        return result;
    }

    public static int NextNumber() => Randomizer.Next();

    public static string NextString() => Randomizer.GetString();
    public static string NextString(int outputLength, string allowedChars) => Randomizer.GetString(outputLength, allowedChars);

    public static string NextPrivateKeyPem()
    {
        using var rsa = RSA.Create(2048);
        return rsa.ExportRSAPrivateKeyPem();
    }
    
    public static string NextPublicKeyPem()
    {
        using var rsa = RSA.Create(2048);
        return rsa.ExportRSAPublicKeyPem();
    }

    public static string NextAudience()
    {
        var length = Randomizer.Next(12, 32);
        return Randomizer.GetString(length);
    }

    public static HttpContext CreateHttpContextForUser(Guid accountId)
    {
        var httpContextForUser = Substitute.For<HttpContext>();
        httpContextForUser.User = new GenericPrincipal(new GenericIdentity(accountId.ToString()), null);
        return httpContextForUser;
    }
}