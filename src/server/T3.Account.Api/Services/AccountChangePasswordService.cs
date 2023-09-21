using T3.Account.Api.Errors;
using T3.Account.Api.Models;
using T3.Account.Api.Repositories;
using T3.Account.Api.Values;

namespace T3.Account.Api.Services;

public interface IAccountChangePasswordService
{
    /// <summary>
    /// Changes the password of an account
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request);
}

/// <inheritdoc cref="IAccountChangePasswordService"/>
public class AccountChangePasswordService : IAccountChangePasswordService
{
    private readonly IPasswordService _passwordService;
    private readonly IAccountRepository _repository;

    public AccountChangePasswordService(IAccountRepository repository, IPasswordService passwordService)
    {
        _repository = repository;
        _passwordService = passwordService;
    }

    public async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request)
    {
        if(request.NewPassword == string.Empty) throw new ChangePasswordException("New password cannot be empty");
        
        var account = await _repository.FindByUsername(request.Username);
        if(account == null) throw new ChangePasswordException("Account not found");
        var oldPasswordValue = PasswordValue.Parse(account.PasswordHash);
        if (!_passwordService.Verify(request.OldPassword, oldPasswordValue)) throw new InvalidPasswordException("Old password is invalid");

        var newPasswordValue = _passwordService.Hash(request.NewPassword);
        
        account.PasswordHash = newPasswordValue.ToSerializedString();
        await _repository.UpdateOne(account);
        return new ChangePasswordResponse();
    }
}