using Microsoft.Extensions.Options;
using NSubstitute;
using T3.Account.Api.Services;

namespace T3.Account.Api.Test.Services;

public class PasswordV1ServiceTests
{
    [Test]
    public void Test_Hash()
    {
        var settings = new PasswordV1Service.Settings()
        {
            Iterations = 1000,
            SaltSize = 16,
            HashSize = 64
        };

        var settingsMock = Substitute.For<IOptions<PasswordV1Service.Settings>>();
        settingsMock.Value.Returns(settings);

        var sut = new PasswordV1Service(settingsMock);
        var rawPassword = "password";
        var hashedPassword = sut.Hash(rawPassword);

        Assert.That(hashedPassword.Version, Is.EqualTo(PasswordV1Service.Version));
        Assert.That(hashedPassword.Iterations, Is.EqualTo(settings.Iterations));
        Assert.That(hashedPassword.Hash.Length, Is.EqualTo(settings.HashSize));
        Assert.That(hashedPassword.Salt.Length, Is.EqualTo(settings.SaltSize));
        Assert.That(hashedPassword.Options, Is.Null.Or.Empty, "V1 has no options");
        TestContext.WriteLine("HashedPassword: " + hashedPassword.ToSerializedString());
    }

    [Test]
    public void Test_Verify()
    {
        var settings = new PasswordV1Service.Settings()
        {
            Iterations = 1000,
            SaltSize = 16,
            HashSize = 64
        };
        var settingsMock = Substitute.For<IOptions<PasswordV1Service.Settings>>();
        settingsMock.Value.Returns(settings);

        var sut = new PasswordV1Service(settingsMock);
        var rawPassword = "password";
        var hashedPassword = sut.Hash(rawPassword);

        Assert.That(sut.Verify(rawPassword, hashedPassword), Is.True);
        TestContext.WriteLine("HashedPassword: " + hashedPassword.ToSerializedString());
    }

    [Test]
    public void Test_Verify_Second_Instance()
    {
        var settings1 = new PasswordV1Service.Settings()
        {
            Iterations = TestContext.CurrentContext.Random.Next(500, 1000),
            SaltSize = TestContext.CurrentContext.Random.Next(8, 32),
            HashSize = TestContext.CurrentContext.Random.Next(16, 64)
        };
        var settingsMock1 = Substitute.For<IOptions<PasswordV1Service.Settings>>();
        settingsMock1.Value.Returns(settings1);
        var sut1 = new PasswordV1Service(settingsMock1);

        // The second instance has different settings
        var settings2 = new PasswordV1Service.Settings()
        {
            Iterations = settings1.Iterations + 10,
            SaltSize = settings1.Iterations + 10,
            HashSize = settings1.HashSize + 10
        };

        var settingsMock2 = Substitute.For<IOptions<PasswordV1Service.Settings>>();
        settingsMock2.Value.Returns(settings2);
        var sut2 = new PasswordV1Service(settingsMock2);

        var rawPassword = TestContext.CurrentContext.Random.GetString();

        var hashedPassword = sut1.Hash(rawPassword);

        Assert.That(sut2.Verify(rawPassword, hashedPassword), Is.True);
        TestContext.WriteLine("HashedPassword: " + hashedPassword.ToSerializedString());
    }
}