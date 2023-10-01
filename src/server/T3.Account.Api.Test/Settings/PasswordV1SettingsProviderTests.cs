using Microsoft.Extensions.Configuration;
using NSubstitute;
using T3.Account.Api.Errors;
using T3.Account.Api.Settings;

namespace T3.Account.Api.Test.Settings;

public class PasswordV1SettingsProviderTests
{
    IConfiguration _configurationFake = Substitute.For<IConfiguration>();

    [SetUp]
    public void SetUp()
    {
        _configurationFake = Substitute.For<IConfiguration>();
    }
    
    [Test]
    public void Properties__returns_value_from_configuration()
    {
        var saltSize = RandomData.NextNumber();
        var hashSize = RandomData.NextNumber();
        var iterations = RandomData.NextNumber();
        _configurationFake["PasswordV1Settings:SaltSize"].Returns(saltSize.ToString());
        _configurationFake["PasswordV1Settings:HashSize"].Returns(hashSize.ToString());
        _configurationFake["PasswordV1Settings:Iterations"].Returns(iterations.ToString());
        var sut = new PasswordV1SettingsProvider(_configurationFake);
        Assert.That(sut.SaltSize, Is.EqualTo(saltSize));
        Assert.That(sut.HashSize, Is.EqualTo(hashSize));
        Assert.That(sut.Iterations, Is.EqualTo(iterations));
    }

    [Test]
    public void Constructor__throws_if_saltSize_is_null()
    {
        var hashSize = RandomData.NextNumber();
        var iterations = RandomData.NextNumber();
        _configurationFake["PasswordV1Settings:SaltSize"].Returns((string)null!);
        _configurationFake["PasswordV1Settings:HashSize"].Returns(hashSize.ToString());
        _configurationFake["PasswordV1Settings:Iterations"].Returns(iterations.ToString());
        
        Assert.That(()=> new PasswordV1SettingsProvider(_configurationFake), Throws.TypeOf<ConfigurationException>().With.Message.Contains("Missing SaltSize"));
    }
    
    [Test]
    public void Constructor__throws_if_hashSize_is_null()
    {
        var saltSize = RandomData.NextNumber();
        var iterations = RandomData.NextNumber();
        _configurationFake["PasswordV1Settings:SaltSize"].Returns(saltSize.ToString());
        _configurationFake["PasswordV1Settings:HashSize"].Returns((string)null!);
        _configurationFake["PasswordV1Settings:Iterations"].Returns(iterations.ToString());
        
        Assert.That(()=> new PasswordV1SettingsProvider(_configurationFake), Throws.TypeOf<ConfigurationException>().With.Message.Contains("Missing HashSize"));
    }
    
    [Test]
    public void Constructor__throws_if_iterations_is_null()
    {
        var saltSize = RandomData.NextNumber();
        var hashSize = RandomData.NextNumber();
        _configurationFake["PasswordV1Settings:SaltSize"].Returns(saltSize.ToString());
        _configurationFake["PasswordV1Settings:HashSize"].Returns(hashSize.ToString());
        _configurationFake["PasswordV1Settings:Iterations"].Returns((string)null!);
        Assert.That(()=> new PasswordV1SettingsProvider(_configurationFake), Throws.TypeOf<ConfigurationException>().With.Message.Contains("Missing Iterations"));
    }

}