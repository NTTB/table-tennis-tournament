using NSubstitute;
using T3.Account.Api.Models;
using T3.Account.Api.Services;

namespace T3.Account.Api.Test.Services;

public class RemoteTokenGeneratorTests
{
    private IRemoteTokenGenerator _sut = Substitute.For<IRemoteTokenGenerator>();
    private IAccountWebTokenGenerator _accountWebTokenGenerator = Substitute.For<IAccountWebTokenGenerator>();
    
    [SetUp]
    public void SetUp()
    {
        _accountWebTokenGenerator = Substitute.For<IAccountWebTokenGenerator>();
        _sut = new RemoteTokenGenerator(_accountWebTokenGenerator);
    }

    [Test]
    public void Test()
    {
        // Arrange
        // The identity is determined by the JWT token, which is not available in the test and is guarded by the [Authorize] attribute.
        var accountId = RandomData.NextNameIdentifier();
        var username = RandomData.NextUsername();
        var accountInfo = new AccountInfo(accountId, username);
        var remoteAudience = RandomData.NextAudience();
        var validToken = $"valid-token [{RandomData.NextString()}]";

        _accountWebTokenGenerator.Generate(accountInfo, remoteAudience).Returns(validToken);
        
        var request = new RemoteTokenRequest { RemoteAudience = remoteAudience };
        
        // Act
        var response =  _sut.Generate(request, accountInfo);
        
        // Assert
        Assert.That(response.Token, Is.EqualTo(validToken));
    }
}