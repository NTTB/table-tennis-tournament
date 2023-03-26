using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using T3.Web.Services.Identity;

namespace T3.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountCreateService _accountCreateService;
    private readonly IAccountLoginService _accountLoginService;

    public AccountController(
        IAccountCreateService accountCreateService,
        IAccountLoginService accountLoginService)
    {
        _accountCreateService = accountCreateService;
        _accountLoginService = accountLoginService;
    }

    [HttpPost("create")]
    public async Task<AccountCreateResponse> Create([FromBody] AccountCreateRequest request)
    {
        return await _accountCreateService.CreateNew(request);
    }

    [HttpPost("login")]
    public async Task<AccountLoginResponse> Login([FromBody] AccountLoginRequest request)
    {
        return await _accountLoginService.Login(request);
    }
}