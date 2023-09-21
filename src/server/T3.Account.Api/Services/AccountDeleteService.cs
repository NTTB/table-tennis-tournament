using T3.Account.Api.Errors;
using T3.Account.Api.Models;
using T3.Account.Api.Repositories;

namespace T3.Account.Api.Services;

public interface IAccountDeleteService
{
    Task Delete(DeleteAccountRequest request);
}

public class AccountDeleteService : IAccountDeleteService
{
    private readonly IAccountRepository _accountRepository;

    public AccountDeleteService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task Delete(DeleteAccountRequest request)
    {
        var account = await _accountRepository.FindByGuid(request.AccountId);
        if (account == null) throw new DeleteAccountException("Account not found");
        await _accountRepository.DeleteOne(account);
    }
}