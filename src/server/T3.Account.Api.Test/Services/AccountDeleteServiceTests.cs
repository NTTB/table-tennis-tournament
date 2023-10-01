using NSubstitute;
using T3.Account.Api.Entities;
using T3.Account.Api.Errors;
using T3.Account.Api.Models;
using T3.Account.Api.Repositories;
using T3.Account.Api.Services;

namespace T3.Account.Api.Test.Services;

public class AccountDeleteServiceTests
{
    private IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private IAccountDeleteService _sut = Substitute.For<IAccountDeleteService>();

    [SetUp]
    public void Setup()
    {
        _accountRepository = Substitute.For<IAccountRepository>();
        _sut = new AccountDeleteService(_accountRepository);
    }

    [Test]
    public async Task Delete__calls_repository()
    {
        var accountId = Guid.NewGuid();
        var accountEntity = Substitute.For<AccountEntity>();
        _accountRepository.FindByGuid(accountId).Returns(accountEntity);

        var request = new DeleteAccountRequest(accountId);
        await _sut.Delete(request);
        await _accountRepository.Received(1).DeleteOne(accountEntity);
    }

    [Test]
    public void Delete__throws_if_account_is_missing()
    {
        var accountId = Guid.NewGuid();
        _accountRepository.FindByGuid(accountId).Returns((AccountEntity?)null);

        var request = new DeleteAccountRequest(accountId);
        Assert.That(async () => await _sut.Delete(request), Throws.TypeOf<DeleteAccountException>().And.Message.Contains("Account not found"));
    }
}