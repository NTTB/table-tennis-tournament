using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using T3.Account.Api.Errors;
using T3.Account.Api.Models;
using T3.Account.Api.Services;

namespace T3.Account.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountCreateService _accountCreateService;
    private readonly IAccountChangePasswordService _accountChangePasswordService;
    private readonly IAccountDeleteService _accountDeleteService;
    private readonly IAccountLoginService _accountLoginService;
    private readonly IRemoteTokenGenerator _remoteTokenGenerator;
    private readonly IAccountInfoTransformer _accountInfoTransformer;

    public AccountController(
        IAccountCreateService accountCreateService,
        IAccountChangePasswordService accountChangePasswordService, 
        IAccountDeleteService accountDeleteService, 
        IAccountLoginService accountLoginService, 
        IRemoteTokenGenerator remoteTokenGenerator,
        IAccountInfoTransformer accountInfoTransformer)
    {
        _accountCreateService = accountCreateService;
        _accountChangePasswordService = accountChangePasswordService;
        _accountDeleteService = accountDeleteService;
        _accountLoginService = accountLoginService;
        _remoteTokenGenerator = remoteTokenGenerator;
        _accountInfoTransformer = accountInfoTransformer;
    }

    [HttpPost]
    public async Task<CreateAccountResponse> Create(CreateAccountRequest request) => await _accountCreateService.Create(request);
    
    [Authorize]
    [HttpPost("change-password")]
    public async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request)
    {
        return await _accountChangePasswordService.ChangePassword(request);
    }

    [Authorize]
    [HttpDelete("{accountId:guid}")]
    public async Task Delete(Guid accountId) 
    {
        if(User.FindFirst(ClaimTypes.NameIdentifier)?.Value != accountId.ToString())
            throw new HttpRequestException("You are not authorized to delete this account", null, HttpStatusCode.Forbidden);

        await _accountDeleteService.Delete(new DeleteAccountRequest(accountId));
    }
    
    [HttpPost("login")]
    public async Task<LoginResponse> Login(LoginRequest request)
    {
        try
        {
            return await _accountLoginService.Login(request);
        }
        catch (LoginException ex)
        {
            throw new HttpRequestException("Username or password is incorrect", ex, HttpStatusCode.BadRequest);
        }
    }

    [Authorize]
    [HttpPost("login-remote")]
    public RemoteTokenResponse CreateRemoteToken(RemoteTokenRequest request)
    {
        var accountInfo = _accountInfoTransformer.FromClaimsPrincipal(User);
        return _remoteTokenGenerator.Generate(request, accountInfo);
    }
}