using System.Security.Claims;

namespace T3.Account.Api.Services;

public interface IAccountInfoTransformer
{
    AccountInfo FromClaimsPrincipal(ClaimsPrincipal claimsPrincipal);
}

public class AccountInfoTransformer : IAccountInfoTransformer
{
    public AccountInfo FromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        var subject = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var givenName = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        return new AccountInfo(subject, givenName);
    }
}