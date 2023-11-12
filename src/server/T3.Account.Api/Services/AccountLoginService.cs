using T3.Account.Api.Errors;
using T3.Account.Api.Models;
using T3.Account.Api.Repositories;
using T3.Account.Api.Values;

namespace T3.Account.Api.Services;

public interface IAccountLoginService
{
    /// <summary>
    /// Generates a JWT for the given account
    /// </summary>
    /// <param name="request">The username, password and audience</param>
    /// <returns>A response that contains a token or throws LoginException</returns>
    Task<LoginResponse> Login(LoginRequest request);
}

public class AccountLoginService : IAccountLoginService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountWebTokenGenerator _accountWebTokenGenerator;
    private readonly IPasswordService _passwordService;

    public AccountLoginService(IAccountRepository accountRepository, IAccountWebTokenGenerator accountWebTokenGenerator, IPasswordService passwordService)
    {
        _accountRepository = accountRepository;
        _accountWebTokenGenerator = accountWebTokenGenerator;
        _passwordService = passwordService;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var account = await _accountRepository.FindByUsername(request.Username);
        if (account == null) throw new LoginException("Username not found");
        
        // Verify password
        var passwordValue = PasswordValue.Parse(account.PasswordHash);
        if(!_passwordService.Verify(request.Password, passwordValue)) throw new LoginException("Failed to verify password");

        var token = _accountWebTokenGenerator.Generate(new AccountInfo(account.Id.ToString(), account.Username));
        return new LoginResponse(token);
    }
}