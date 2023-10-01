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
    
    [TestCase("")]
    [TestCase("v1")]
    [TestCase("v1$")]
    [TestCase("v1$1000$")]
    [TestCase("v1$1000$DEAD")]
    public void Parse__throws_if_values_are_missing(string input)
    {
        Assert.That(() => PasswordValue.Parse(input), Throws.Exception.With.Message.Contains("The input string is not valid. It must contain at least 4 values separated by '$'."));
    }

    [Test]
    public void ToSerializedString__throws_if_version_has_dollar_sign()
    {
        var sut =new PasswordValue()
        {
            Version = "$v1",
            Iterations = RandomData.NextNumber(),
            Salt = RandomData.NextByteArray(100),
            Hash = RandomData.NextByteArray(100),
            Options = null
        };
        
        Assert.That(()=> sut.ToSerializedString(), Throws.Exception.With.Message.Contains("The value '$v1' contains a '$' which is not allowed in the Version parameter."));
    }
    
    
    [Test]
    public void ToSerializedString__throws_if_option_has_dollar_sign()
    {
        var sut =new PasswordValue()
        {
            Version = "v1",
            Iterations = RandomData.NextNumber(),
            Salt = RandomData.NextByteArray(100),
            Hash = RandomData.NextByteArray(100),
            Options = new string[]{ "$sign" }
        };
        
        Assert.That(()=> sut.ToSerializedString(), Throws.Exception.With.Message.Contains("The value '$sign' contains a '$' which is not allowed in the options[0] parameter."));
    }
    
        
    [Test]
    public void ToSerializedString__throws_if_another_option_has_dollar_sign()
    {
        var sut =new PasswordValue()
        {
            Version = "v1",
            Iterations = RandomData.NextNumber(),
            Salt = RandomData.NextByteArray(100),
            Hash = RandomData.NextByteArray(100),
            Options = new string[]{ RandomData.NextString(10, "abcdefghijklmnopqrstuvwxyz"), "$sign" }
        };
        
        Assert.That(()=> sut.ToSerializedString(), Throws.Exception.With.Message.Contains("The value '$sign' contains a '$' which is not allowed in the options[1] parameter."));
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