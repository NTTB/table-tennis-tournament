using System.Security.Principal;
using T3.Account.Api.Models;

namespace T3.Account.Api.Services;

public interface IRemoteTokenGenerator
{
    RemoteTokenResponse Generate(RemoteTokenRequest request, AccountInfo accountInfo);
}

public class RemoteTokenGenerator : IRemoteTokenGenerator
{
    private readonly IAccountWebTokenGenerator _accountWebTokenGenerator;

    public RemoteTokenGenerator(IAccountWebTokenGenerator accountWebTokenGenerator)
    {
        _accountWebTokenGenerator = accountWebTokenGenerator;
    }

    public RemoteTokenResponse Generate(RemoteTokenRequest request, AccountInfo accountInfo)
    {
        var token = _accountWebTokenGenerator.Generate(accountInfo, request.RemoteAudience);
        return new RemoteTokenResponse(token);
    }
}