using System.Text;
using T3.Account.Api.Values;

namespace T3.Account.Api.Test.Services;

public class PasswordValueTests
{
    [TestOf(nameof(PasswordValue))]
    [TestCaseSource(nameof(ToSerializedStringCases))]
    public string TestToSerializedString(string version, int iterations, byte[] salt, byte[] hash,
        string[] additionalOptions)
    {
        return new PasswordValue()
        {
            Version = version,
            Iterations = iterations,
            Salt = salt,
            Options = additionalOptions,
            Hash = hash
        }.ToSerializedString();
    }

    public static IEnumerable<TestCaseData> ToSerializedStringCases()
    {
        // Helper methods
        byte[] ToBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        string ToBytesBase64(string s)
        {
            return Convert.ToBase64String(ToBytes(s));
        }

        yield return new TestCaseData("v1", 1000, ToBytes("salt"), ToBytes("hash"), Array.Empty<string>())
            .Returns($"v1$1000${ToBytesBase64("salt")}${ToBytesBase64("hash")}")
            .SetName("Check with minimal case values");

        var randomSalt = TestContext.CurrentContext.Random.GetString(20, "random_salt");
        var randomHash = TestContext.CurrentContext.Random.GetString(20, "random_hash");
        yield return new TestCaseData("v2", 1234, ToBytes(randomSalt), ToBytes(randomHash),
                new[] { "option1", "option2" })
            .Returns($"v2$1234${ToBytesBase64(randomSalt)}$option1$option2${ToBytesBase64(randomHash)}")
            .SetName("Check with random values");
    }

    [TestOf(nameof(PasswordValue))]
    [TestCaseSource(nameof(ParseCases))]
    public void TestParse(string input, string version, int iterations, byte[] salt, byte[] hash,
        string[] additionalOptions)
    {
        var sut = PasswordValue.Parse(input);

        Assert.Multiple(() =>
        {
            Assert.That(sut.Version, Is.EqualTo(version), "version is not equal");
            Assert.That(sut.Iterations, Is.EqualTo(iterations), "iterations is not equal");
            Assert.That(sut.Salt, Is.EqualTo(salt), "salt is not equal");
            Assert.That(sut.Hash, Is.EqualTo(hash), "hash is not equal");

            var actualOptions = sut.Options ?? Array.Empty<string>();
            Assert.That(actualOptions.Length, Is.EqualTo(additionalOptions.Length), "The number of options is not equal");
            if (additionalOptions.Length == actualOptions.Length)
                for (var index = 0; index < additionalOptions.Length; index++)
                {
                    var expectedOption = additionalOptions[index];
                    var actualOption = actualOptions[index];
                    Assert.That(actualOption, Is.EqualTo(expectedOption), $"The option at index {index} is not equal");
                }
        });
    }

    public static IEnumerable<TestCaseData> ParseCases()
    {
        // Helper methods
        byte[] ToBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        string ToBytesBase64(string s)
        {
            return Convert.ToBase64String(ToBytes(s));
        }

        yield return new TestCaseData(
            $"v1$1000${ToBytesBase64("salt")}${ToBytesBase64("hash")}",
            "v1", 1000, ToBytes("salt"), ToBytes("hash"), Array.Empty<string>()
        ).SetName("Check with minimal case values");

        var randomSalt = TestContext.CurrentContext.Random.GetString(20, "random_salt");
        var randomHash = TestContext.CurrentContext.Random.GetString(20, "random_hash");
        yield return new TestCaseData(
                $"v2$1234${ToBytesBase64(randomSalt)}$option1$option2${ToBytesBase64(randomHash)}",
                "v2", 1234, ToBytes(randomSalt), ToBytes(randomHash), new[] { "option1", "option2" })
            .SetName("Check with random values");
    }
}