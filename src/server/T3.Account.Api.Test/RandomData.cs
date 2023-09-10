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
}