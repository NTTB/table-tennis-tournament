using Microsoft.AspNetCore.Components.RenderTree;
using MongoDB.Driver;
using T3.Account.Api.Entities;
using T3.Account.Api.Errors;
using T3.Account.Api.Models;
using T3.Account.Api.Repositories;

namespace T3.Account.Api.Services;

/// <summary>
/// The account service is responsible for managing accounts.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Create and stores a new account
    /// </summary>
    /// <param name="request">The details of the request</param>
    /// <returns>The create account entity</returns>
    Task<AccountEntity> Create(CreateAccountRequest request);
}

/// <inheritdoc cref="IAccountService"/>
public class AccountService : IAccountService
{
    private readonly IPasswordService _passwordService;
    private readonly IAccountRepository _repository;

    public AccountService(IAccountRepository repository, IPasswordService passwordService)
    {
        _repository = repository;
        _passwordService = passwordService;
    }

    public async Task<AccountEntity> Create(CreateAccountRequest request)
    {
        if (request.Username == string.Empty) throw new CreateAccountException("Username cannot be empty");
        if (request.Password == string.Empty) throw new CreateAccountException("Password cannot be empty");
        if (await IsExistingUsername(request.Username)) throw new CreateAccountException("Username already in use");

        var password = _passwordService.Hash(request.Password);
        var accountEntity = new AccountEntity(Guid.NewGuid(), request.Username, password.ToSerializedString());
        await _repository.InsertOne(accountEntity);
        return accountEntity;
    }

    private async Task<bool> IsExistingUsername(string username)
    {
        return (await _repository.FindByUsername(username)) != null;
    }
}