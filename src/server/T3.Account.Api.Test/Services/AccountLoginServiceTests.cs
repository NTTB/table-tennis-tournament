using NSubstitute;
using T3.Account.Api.Entities;
using T3.Account.Api.Errors;
using T3.Account.Api.Models;
using T3.Account.Api.Repositories;
using T3.Account.Api.Services;
using T3.Account.Api.Values;

namespace T3.Account.Api.Test.Services;

public class AccountLoginServiceTests
{
    private IAccountLoginService _sut = Substitute.For<IAccountLoginService>();
    private IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private IAccountWebTokenGenerator _accountWebTokenGenerator = Substitute.For<IAccountWebTokenGenerator>();
    private IPasswordService _passwordService = Substitute.For<IPasswordService>();

    [SetUp]
    public void SetUp()
    {
        _accountRepository = Substitute.For<IAccountRepository>();
        _accountWebTokenGenerator = Substitute.For<IAccountWebTokenGenerator>();
        _passwordService = Substitute.For<IPasswordService>();
        _sut = new AccountLoginService(_accountRepository, _accountWebTokenGenerator, _passwordService);
    }

    [TearDown]
    public void TearDown()
    {
    }

    [Test]
    public void Login__throws_if_account_does_not_exist()
    {
        var username = RandomData.NextUsername();
        var password = RandomData.NextPassword();
        var audience = RandomData.NextAudience();
        Assert.That(
            async () => await _sut.Login(new LoginRequest(username, password)),
            Throws.TypeOf<LoginException>().With.Message.Contains("Username not found")
        );
    }

    [Test]
    public async Task Login__returns_token_if_account_exists()
    {
        var username = RandomData.NextUsername();
        var password = RandomData.NextPassword();
        var audience = RandomData.NextAudience();

        var newGuid = Guid.NewGuid();
        var nextPasswordValue = RandomData.NextPasswordValue();
        var validAccount = new AccountEntity()
        {
            Id = newGuid,
            Username = username,
            PasswordHash = nextPasswordValue.ToSerializedString()
        };

        _accountRepository.FindByUsername(username)!.Returns(Task.FromResult(validAccount));
        _accountWebTokenGenerator
            .Generate(Arg.Is<AccountInfo>(x => x.Subject == newGuid.ToString() && x.GivenName == username))
            .Returns("fake-jwt-token");

        _passwordService.Verify(password, Arg.Any<PasswordValue>()).Returns(true);

        var response = await _sut.Login(new LoginRequest(username, password));
        Assert.That(response.Token, Is.EqualTo("fake-jwt-token"));
    }
    
    [Test]
    public void Login__throws_if_password_does_not_match()
    {
        var username = RandomData.NextUsername();
        var password = RandomData.NextPassword();
        var audience = RandomData.NextAudience();

        var newGuid = Guid.NewGuid();
        var nextPasswordValue = RandomData.NextPasswordValue();
        var validAccount = new AccountEntity()
        {
            Id = newGuid,
            Username = username,
            PasswordHash = nextPasswordValue.ToSerializedString()
        };

        _accountRepository.FindByUsername(username)!.Returns(Task.FromResult(validAccount));
        _passwordService.Verify(password, Arg.Any<PasswordValue>()).Returns(false);

        Assert.That(
            async () => await _sut.Login(new LoginRequest(username, password)),
            Throws.TypeOf<LoginException>().With.Message.Contains("Failed to verify password")
        );
    }
}