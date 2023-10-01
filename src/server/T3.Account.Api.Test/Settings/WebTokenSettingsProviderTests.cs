using Microsoft.Extensions.Configuration;
using NSubstitute;
using T3.Account.Api.Errors;
using T3.Account.Api.Settings;

namespace T3.Account.Api.Test.Settings;

public class WebTokenSettingsProviderTests
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
        var privateKeyPathValue = RandomData.NextString();
        var publicKeyPathValue = RandomData.NextString();
        _configurationFake["WebTokenSettings:PrivateKeyPath"].Returns(privateKeyPathValue);
        _configurationFake["WebTokenSettings:PublicKeyPath"].Returns(publicKeyPathValue);
        var sut = new WebTokenSettingsProvider(_configurationFake);
        Assert.That(sut.PrivateKeyPath, Is.EqualTo(privateKeyPathValue));
        Assert.That(sut.PublicKeyPath, Is.EqualTo(publicKeyPathValue));
    }

    [Test]
    public void Constructor__throws_if_privateKeyPathValue_is_null()
    {
        var privateKeyPathValue = RandomData.NextString();
        var publicKeyPathValue = RandomData.NextString();
        _configurationFake["WebTokenSettings:PrivateKeyPath"].Returns((string?)null);
        _configurationFake["WebTokenSettings:PublicKeyPath"].Returns(publicKeyPathValue);
        
        Assert.That(()=> new WebTokenSettingsProvider(_configurationFake), Throws.TypeOf<ConfigurationException>().With.Message.Contains("Missing PrivateKeyPath"));
    }
    
    [Test]
    public void Constructor__throws_if_publicKeyPathValue_is_null()
    {
        var privateKeyPathValue = RandomData.NextString();
        var publicKeyPathValue = RandomData.NextString();
        _configurationFake["WebTokenSettings:PrivateKeyPath"].Returns(privateKeyPathValue);
        _configurationFake["WebTokenSettings:PublicKeyPath"].Returns((string?)null);
        
        Assert.That(()=> new WebTokenSettingsProvider(_configurationFake), Throws.TypeOf<ConfigurationException>().With.Message.Contains("Missing PublicKeyPath"));
    }
}