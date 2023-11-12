using System.Net;
using System.Security.Claims;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using T3.Account.Api.Controllers;
using T3.Account.Api.Errors;
using T3.Account.Api.Models;
using T3.Account.Api.Services;

namespace T3.Account.Api.Test.Controllers;

public class AccountControllerTests
{
    private AccountController _sut = null!;
    private IAccountCreateService _accountCreateService = Substitute.For<IAccountCreateService>();

    private IAccountChangePasswordService _accountChangePasswordService =
        Substitute.For<IAccountChangePasswordService>();

    private IAccountDeleteService _accountDeleteService = Substitute.For<IAccountDeleteService>();
    private IAccountLoginService _accountLoginService = Substitute.For<IAccountLoginService>();
    private IRemoteTokenGenerator _remoteTokenGenerator = Substitute.For<IRemoteTokenGenerator>();
    private IAccountInfoTransformer _accountInfoTransformer = Substitute.For<IAccountInfoTransformer>();

    [SetUp]
    public void SetUp()
    {
        _accountCreateService = Substitute.For<IAccountCreateService>();
        _accountChangePasswordService = Substitute.For<IAccountChangePasswordService>();
        _accountDeleteService = Substitute.For<IAccountDeleteService>();
        _accountLoginService = Substitute.For<IAccountLoginService>();
        _remoteTokenGenerator = Substitute.For<IRemoteTokenGenerator>();
        _accountInfoTransformer = Substitute.For<IAccountInfoTransformer>();
        

        _sut = new AccountController(
            _accountCreateService,
            _accountChangePasswordService,
            _accountDeleteService,
            _accountLoginService,
            _remoteTokenGenerator,
            _accountInfoTransformer
        );
    }

    [Test]
    public async Task CreateAccount__should_invoke_create_account_service()
    {
        // Arrange
        var username = RandomData.NextUsername();
        var password = RandomData.NextPassword();
        var newAccountId = Guid.NewGuid();

        var request = new CreateAccountRequest(username, password);
        var expectedResponse = new CreateAccountResponse(newAccountId);
        _accountCreateService.Create(request).Returns(expectedResponse);

        // Act
        var response = await _sut.Create(request);

        // Assert
        Assert.That(response.AccountId, Is.EqualTo(newAccountId));
    }

    [Test]
    public async Task ChangePassword__should_invoke_change_password_service()
    {
        // Arrange
        var username = RandomData.NextUsername();
        var newPassword = RandomData.NextPassword();
        var oldPassword = RandomData.NextPassword();

        var request = new ChangePasswordRequest(username, oldPassword, newPassword);
        var expectedResponse = new ChangePasswordResponse();
        _accountChangePasswordService.ChangePassword(request).Returns(expectedResponse);

        // Act
        var response = await _sut.ChangePassword(request);

        // Assert
        Assert.That(response, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task DeleteAccount__should_invoke_delete_account_service()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = new DeleteAccountRequest(accountId);
        var expectedResponse = Task.CompletedTask;
        _sut.ControllerContext.HttpContext = RandomData.CreateHttpContextForUser(accountId, RandomData.NextUsername());
        _accountDeleteService.Delete(request).Returns(expectedResponse);

        // Act
        await _sut.Delete(accountId);

        // Assert
        await _accountDeleteService.Received(1).Delete(request);
    }

    [Test]
    public Task DeleteAccount__throws_if_user_is_not_logged_in()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var wrongAccountId = Guid.NewGuid();

        // Set fake user
        _sut.ControllerContext.HttpContext = RandomData.CreateHttpContextForUser(wrongAccountId, RandomData.NextUsername());

        // Act & Assert
        Assert.That(
            async () => await _sut.Delete(accountId),
            Throws.InstanceOf<HttpRequestException>()
                .With.Property(nameof(HttpRequestException.StatusCode)).EqualTo(HttpStatusCode.Forbidden)
                .With.Message.EqualTo("You are not authorized to delete this account")
        );
        return Task.CompletedTask;
    }

    [Test]
    public async Task Login__returns_a_valid_response()
    {
        var username = RandomData.NextUsername();
        var password = RandomData.NextPassword();
        var audience = RandomData.NextAudience();

        var expectedResponse = new LoginResponse("valid-token");
        var request = new LoginRequest(username, password);

        _accountLoginService.Login(request).Returns(Task.FromResult(expectedResponse));

        var result = await _sut.Login(request);
        Assert.That(result, Is.EqualTo(expectedResponse));
    }

    [Test]
    public void Login__throws_if_login_fails()
    {
        var username = RandomData.NextUsername();
        var password = RandomData.NextPassword();
        var audience = RandomData.NextAudience();

        var expectedResponse = new LoginResponse("valid-token");
        var request = new LoginRequest(username, password);

        var innerException = new LoginException("Login failure message");
        _accountLoginService.Login(request).ThrowsAsync(innerException);

        var ex = Assert.CatchAsync<HttpRequestException>(async () => await _sut.Login(request));
        Assert.Multiple(() =>
        {
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(ex?.Message, Is.EqualTo("Username or password is incorrect"));
            Assert.That(ex?.InnerException, Is.EqualTo(innerException));
        });
    }

    [Test]
    public void CreateRemoteToken__returns_a_valid_token()
    {
        var accountId = Guid.NewGuid();
        var audience = RandomData.NextAudience();
        var username = RandomData.NextUsername();
        var accountInfo = new AccountInfo(accountId.ToString(), username);

        var expectedResponse = new RemoteTokenResponse("valid-token");

        _sut.ControllerContext.HttpContext = RandomData.CreateHttpContextForUser(accountId, username);
        _accountInfoTransformer.FromClaimsPrincipal(Arg.Any<ClaimsPrincipal>()).Returns(accountInfo);
        _remoteTokenGenerator.Generate(
            Arg.Is<RemoteTokenRequest>(x => x.RemoteAudience == audience),
            Arg.Is(accountInfo)
        ).Returns(expectedResponse);

        var result =  _sut.CreateRemoteToken(new RemoteTokenRequest() { RemoteAudience = audience });
        Assert.That(result, Is.EqualTo(expectedResponse));
    }
}