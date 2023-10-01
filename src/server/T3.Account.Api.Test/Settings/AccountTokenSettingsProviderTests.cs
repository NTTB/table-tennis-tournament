using Microsoft.Extensions.Configuration;
using NSubstitute;
using T3.Account.Api.Errors;
using T3.Account.Api.Settings;

namespace T3.Account.Api.Test.Settings;

public class AccountTokenSettingsProviderTests
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
        var issuer = RandomData.NextString();
        _configurationFake["AccountTokenSettings:Issuer"].Returns(issuer);
        var sut = new AccountTokenSettingsProvider(_configurationFake);
        Assert.That(sut.Issuer, Is.EqualTo(issuer));
    }

    [Test]
    public void Constructor__throws_if_privateKeyPathValue_is_null()
    {
        _configurationFake["AccountTokenSettings:Issuer"].Returns((string?)null!);
        Assert.That(()=> new AccountTokenSettingsProvider(_configurationFake), Throws.TypeOf<ConfigurationException>().With.Message.Contains("Missing Issuer"));
    }
}