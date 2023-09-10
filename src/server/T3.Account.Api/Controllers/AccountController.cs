using Microsoft.AspNetCore.Mvc;
using T3.Account.Api.Models;

namespace T3.Users.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<CreateAccountResponse> Create(CreateAccountRequest request) => throw new NotImplementedException();
    
    [HttpGet("{accountId:guid}")]
    public async Task Get(Guid accountId) => throw new NotImplementedException();
    
    [HttpPost("{accountId:guid}/change-password")]
    public async Task Change(Guid accountId, ChangePasswordRequest request) => throw new NotImplementedException();

    [HttpDelete("{accountId:guid}")]
    public async Task Delete(Guid accountId) => throw new NotImplementedException();

    [HttpPost("login")]
    public async Task<LoginResponse> Login(LoginRequest request) => throw new NotImplementedException();
}