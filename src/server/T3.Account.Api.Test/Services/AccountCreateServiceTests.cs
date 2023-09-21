using NSubstitute;
using T3.Account.Api.Entities;
using T3.Account.Api.Errors;
using T3.Account.Api.Models;
using T3.Account.Api.Repositories;
using T3.Account.Api.Services;

namespace T3.Account.Api.Test.Services;

public class AccountCreateServiceTests
{
    private IAccountCreateService _sut = null!;
    private IPasswordService _passwordService = null!;
    private IAccountRepository _accountRepository = null!;

    [SetUp]
    public void Setup()
    {
        _passwordService = Substitute.For<IPasswordService>();
        _accountRepository = Substitute.For<IAccountRepository>();
        _sut = new AccountCreateService(_accountRepository, _passwordService);
    }

    [Test]
    public async Task Create__returns_a_valid_response()
    {
        var request = new CreateAccountRequest(
            RandomData.NextUsername(),
            RandomData.NextPassword()
        );

        var fakePasswordHash = RandomData.NextPasswordValue();
        _passwordService.Hash(request.Password).Returns(fakePasswordHash);

        var result = await _sut.Create(request);


        Assert.That(result.AccountId, Is.Not.EqualTo(Guid.Empty), "The account id should not be empty.");

        await _accountRepository.Received(1).InsertOne(Arg.Is<AccountEntity>(x =>
            x.Username == request.Username && x.PasswordHash == fakePasswordHash.ToSerializedString() && x.Id == result.AccountId));
    }

    [Test]
    public void Create__throws_if_empty_username()
    {
        var request = new CreateAccountRequest(
            string.Empty,
            RandomData.NextPassword()
        );

        Assert.That(() => _sut.Create(request), Throws.TypeOf<CreateAccountException>().And.Message.Contain("Username cannot be empty"));
    }

    [Test]
    public void Create__throws_if_empty_password()
    {
        var request = new CreateAccountRequest(
            RandomData.NextUsername(),
            string.Empty
        );

        Assert.That(() => _sut.Create(request), Throws.TypeOf<CreateAccountException>().And.Message.Contain("Password cannot be empty"));
    }

    [Test]
    public void Create__throws_if_username_already_in_use()
    {
        var username = RandomData.NextUsername();
        var request1 = new CreateAccountRequest(username, RandomData.NextPassword());
        var request2 = new CreateAccountRequest(username, RandomData.NextPassword());

        _passwordService.Hash(Arg.Any<string>()).Returns(RandomData.NextPasswordValue());
        string username1 = string.Empty;
        string passwordHash = string.Empty;
        _accountRepository.FindByUsername(username).Returns(
            Task.FromResult<AccountEntity?>(null), // First response
            Task.FromResult<AccountEntity?>(new AccountEntity
            {
                Id = Guid.Empty,
                Username = username1,
                PasswordHash = passwordHash
            }) // second response
        );

        // First find on account collection returns no results, the second find returns a result.
        Assume.That(async () => await _sut.Create(request1), Throws.Nothing, "The first request should not throw.");
        Assert.That(async () => await _sut.Create(request2), Throws.TypeOf<CreateAccountException>().And.Message.Contain("Username already in use"));
    }
}