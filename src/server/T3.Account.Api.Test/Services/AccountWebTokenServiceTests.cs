using System.Security.Cryptography;
using JWT.Algorithms;
using JWT.Builder;
using NSubstitute;
using T3.Account.Api.Entities;
using T3.Account.Api.Services;
using T3.Account.Api.Settings;

namespace T3.Account.Api.Test.Services;

public class AccountWebTokenServiceTests
{
    private IAccountWebTokenGenerator _sut = Substitute.For<IAccountWebTokenGenerator>();
    private IWebTokenAlgorithmGenerator _webTokenAlgorithmGenerator = Substitute.For<IWebTokenAlgorithmGenerator>();
    private AccountTokenSettings _accountTokenSettings = Substitute.For<AccountTokenSettings>();

    [SetUp]
    public void SetUp()
    {
        _webTokenAlgorithmGenerator = Substitute.For<IWebTokenAlgorithmGenerator>();
        _accountTokenSettings = Substitute.For<AccountTokenSettings>();
        _sut = new AccountWebTokenGenerator(_webTokenAlgorithmGenerator, _accountTokenSettings);
    }
    
    [Test]
    public void GenerateToken__throws_if_account_is_null()
    {
        Assert.That(() => _sut.Generate(null!, RandomData.NextString()), Throws.TypeOf<ArgumentNullException>().And.Message.Contain("AccountEntity cannot be null"));
    }

    [Test]
    public void GenerateToken__returns_jwt_token_of_issuing_server()
    {
        var issuer = RandomData.NextString();
        using var key = RSA.Create(2048);
        var algorithm = new RS384Algorithm(key, key);
        _webTokenAlgorithmGenerator.Algorithm.Returns(algorithm);
        _accountTokenSettings.Issuer.Returns(issuer);

        var audience = RandomData.NextString();
        var givenName = RandomData.NextString();
        var subject = RandomData.NextString();
        var jwtToken =  _sut.Generate(new AccountInfo(subject, givenName));
        Assert.That(jwtToken, Is.Not.Null);

        var parsedOutcome =JwtBuilder.Create().WithAlgorithm(algorithm).Decode(jwtToken);
        
        Assert.Multiple(()=>{
            Assert.That(parsedOutcome, Contains.Substring($"\"given_name\":\"{givenName}\""));
            Assert.That(parsedOutcome, Contains.Substring($"\"sub\":\"{subject}\""));
            Assert.That(parsedOutcome, Contains.Substring($"\"aud\":\"{issuer}\""));
            Assert.That(parsedOutcome, Contains.Substring($"\"iss\":\"{issuer}\""));
        });
    }
    
    [Test]
    public void GenerateToken__returns_jwt_token()
    {
        var issuer = RandomData.NextString();
        using var key = RSA.Create(2048);
        var algorithm = new RS384Algorithm(key, key);
        _webTokenAlgorithmGenerator.Algorithm.Returns(algorithm);
        _accountTokenSettings.Issuer.Returns(issuer);

        var audience = RandomData.NextString();
        var givenName = RandomData.NextString();
        var subject = RandomData.NextString();
        var jwtToken =  _sut.Generate(new AccountInfo(subject, givenName), audience);
        Assert.That(jwtToken, Is.Not.Null);

        var parsedOutcome =JwtBuilder.Create().WithAlgorithm(algorithm).Decode(jwtToken);
        
        Assert.Multiple(()=>{
            Assert.That(parsedOutcome, Contains.Substring($"\"given_name\":\"{givenName}\""));
            Assert.That(parsedOutcome, Contains.Substring($"\"sub\":\"{subject}\""));
            Assert.That(parsedOutcome, Contains.Substring($"\"aud\":\"{audience}\""));
            Assert.That(parsedOutcome, Contains.Substring($"\"iss\":\"{issuer}\""));
        });
    }
}