using T3.Account.Api.Entities;
using T3.Account.Api.Errors;
using T3.Account.Api.Models;
using T3.Account.Api.Repositories;

namespace T3.Account.Api.Services;

public interface IAccountCreateService
{
    Task<CreateAccountResponse> Create(CreateAccountRequest request);
}

public class AccountCreateService : IAccountCreateService
{
    private readonly IPasswordService _passwordService;
    private readonly IAccountRepository _repository;

    public AccountCreateService(IAccountRepository repository, IPasswordService passwordService)
    {
        _repository = repository;
        _passwordService = passwordService;
    }

    public async Task<CreateAccountResponse> Create(CreateAccountRequest request)
    {
        if (request.Username == string.Empty) throw new CreateAccountException("Username cannot be empty");
        if (request.Password == string.Empty) throw new CreateAccountException("Password cannot be empty");
        if (await IsExistingUsername(request.Username)) throw new CreateAccountException("Username already in use");

        var password = _passwordService.Hash(request.Password);
        Guid id = Guid.NewGuid();
        string passwordHash = password.ToSerializedString();
        var accountEntity = new AccountEntity
        {
            Id = id,
            Username = request.Username,
            PasswordHash = passwordHash
        };
        await _repository.InsertOne(accountEntity);
        return new CreateAccountResponse(accountEntity.Id);
    }
    
    private async Task<bool> IsExistingUsername(string username)
    {
        return (await _repository.FindByUsername(username)) != null;
    }
}