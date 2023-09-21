using Microsoft.AspNetCore.Mvc;
using T3.Account.Api.Models;
using T3.Account.Api.Services;

namespace T3.Account.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountCreateService _accountCreateService;
    private readonly IAccountChangePasswordService _accountChangePasswordService;
    private readonly IAccountDeleteService _accountDeleteService;

    public AccountController(ILogger<AccountController> logger, IAccountCreateService accountCreateService, IAccountChangePasswordService accountChangePasswordService, IAccountDeleteService accountDeleteService)
    {
        _logger = logger;
        _accountCreateService = accountCreateService;
        _accountChangePasswordService = accountChangePasswordService;
        _accountDeleteService = accountDeleteService;
    }

    [HttpPost]
    public async Task<CreateAccountResponse> Create(CreateAccountRequest request) => await _accountCreateService.Create(request);
    
    [HttpPost("change-password")]
    public async Task<ChangePasswordResponse> Change(ChangePasswordRequest request) => await _accountChangePasswordService.ChangePassword(request);

    [HttpDelete("{accountId:guid}")]
    public async Task Delete(Guid accountId) 
    {
        // TODO: Add check if user is authorized to delete this account
        await _accountDeleteService.Delete(new DeleteAccountRequest(accountId));
    }

    [HttpPost("login")]
    public async Task<LoginResponse> Login(LoginRequest request) => throw new NotImplementedException();
}