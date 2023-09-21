using NSubstitute;
using T3.Account.Api.Entities;
using T3.Account.Api.Errors;
using T3.Account.Api.Models;
using T3.Account.Api.Repositories;
using T3.Account.Api.Services;
using T3.Account.Api.Values;

namespace T3.Account.Api.Test.Services;

public class AccountChangePasswordServiceTests
{
    private IAccountChangePasswordService _sut = null!;
    private IPasswordService _passwordService = null!;
    private IAccountRepository _accountRepository = null!;

    [SetUp]
    public void Setup()
    {
        _passwordService = Substitute.For<IPasswordService>();
        _accountRepository = Substitute.For<IAccountRepository>();
        _sut = new AccountChangePasswordService(_accountRepository, _passwordService);
    }

    [Test]
    public void ChangePassword__throws_if_password_is_empty()
    {
        var request = new ChangePasswordRequest(
            RandomData.NextUsername(),
            RandomData.NextPassword(),
            string.Empty
        );

        Assert.That(async () => await _sut.ChangePassword(request),
            Throws.TypeOf<ChangePasswordException>().And.Message.Contain("New password cannot be empty"));
    }

    [Test]
    public void ChangePassword__throws_if_user_is_missing()
    {
        var username = RandomData.NextUsername();
        var oldPassword = RandomData.NextPassword();
        var newPassword = RandomData.NextPassword();

        var request = new ChangePasswordRequest(
            username,
            oldPassword,
            newPassword
        );

        _accountRepository.FindByUsername(username).Returns((AccountEntity?)null);

        Assert.That(async () => await _sut.ChangePassword(request), Throws.TypeOf<ChangePasswordException>().And.Message.Contain("Account not found"));
    }

    [Test]
    public void ChangePassword__throws_if_old_password_is_invalid()
    {
        var username = RandomData.NextUsername();
        var oldPassword = RandomData.NextPassword();
        var newPassword = RandomData.NextPassword();

        var request = new ChangePasswordRequest(
            username,
            oldPassword,
            newPassword
        );

        _accountRepository.FindByUsername(username).Returns(new AccountEntity { Username = username, Id = Guid.NewGuid(), PasswordHash = RandomData.NextPasswordValue().ToSerializedString() });
        _passwordService.Verify(oldPassword, Arg.Any<PasswordValue>()).Returns(false);
        Assert.That(async () => await _sut.ChangePassword(request), Throws.TypeOf<InvalidPasswordException>().And.Message.Contain("Old password is invalid"));
    }
    
    [Test]
    public async Task ChangePassword__update_password()
    {
        var username = RandomData.NextUsername();
        var oldPassword = RandomData.NextPassword();
        var newPassword = RandomData.NextPassword();

        var request = new ChangePasswordRequest(
            username,
            oldPassword,
            newPassword
        );

        // Fake data
        var oldHash = RandomData.NextPasswordValue();
        var newHash = RandomData.NextPasswordValue();
        var accountEntity = new AccountEntity { Username = username, Id = Guid.NewGuid(), PasswordHash = oldHash.ToSerializedString() };
        
        _passwordService.Verify(oldPassword, Arg.Any<PasswordValue>()).Returns(true);
        _passwordService.Hash(newPassword).Returns(newHash);
        _accountRepository.FindByUsername(username).Returns(accountEntity);
        _accountRepository.UpdateOne(accountEntity).Returns(Task.CompletedTask);

        // Act
        await _sut.ChangePassword(request);

        // Assert
        await _accountRepository.Received(1).UpdateOne(Arg.Is<AccountEntity>(x => x == accountEntity && x.PasswordHash == newHash.ToSerializedString()));
    }
}