using System.Security.Claims;
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

    public static HttpContext CreateHttpContextForUser(Guid accountId, string username)
    {
        var httpContextForUser = Substitute.For<HttpContext>();
        var identity = new GenericIdentity(string.Empty);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, accountId.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.GivenName, username));
        httpContextForUser.User = new GenericPrincipal(identity, null);
        return httpContextForUser;
    }

    public static string NextNameIdentifier()
    {
        return Guid.NewGuid().ToString();
    }
}