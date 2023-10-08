using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using T3.Web.Services.Identity;

namespace T3.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountPublicKeyService _accountPublicKeyService;
    private readonly IAccountTokenService _accountTokenService;
    private readonly IAccountCreateService _accountCreateService;

    public AccountController(
        IAccountCreateService accountCreateService,
        IAccountPublicKeyService accountPublicKeyService,
        IAccountTokenService accountTokenService)
    {
        _accountPublicKeyService = accountPublicKeyService;
        _accountTokenService = accountTokenService;
        _accountCreateService = accountCreateService;
    }

    [HttpPost("create")]
    public async Task<AccountCreateResponse> Create([FromBody] AccountCreateRequest request)
    {
        return await _accountCreateService.CreateNew(request);
    }
    
    [HttpPost("test")]
    public bool Test([FromBody]TestBody body)
    {
        var jsonWebKey = JsonWebKey.Create(body.PublicKey);
        // Verify the signature of the message that was sent
        var algorithm = CryptoProviderFactory.Default.CreateForVerifying(jsonWebKey, "RS256");
        var isValid = algorithm.Verify(Encoding.UTF8.GetBytes(body.Message), Convert.FromBase64String(body.Signature));
        return isValid;
    }
    
    [Authorize]
    [HttpGet("keys")]
    public async Task<AccountKeysResponse[]> GetKeys()
    {
        var accountId = _accountTokenService.GetIdentity(User).AccountId;
        var exitingKeys = await _accountPublicKeyService.GetKeys(accountId);
        return exitingKeys
            .Select(k => new AccountKeysResponse(k.PublicKey, k.CreatedAtUtc, k.RevokedAtUtc))
            .ToArray();
    }
    
    [Authorize]
    [HttpPost("add-key")]
    public async Task AddKey([FromBody] AccountAddKeyRequest request)
    {
        var accountId = _accountTokenService.GetIdentity(User).AccountId;
        await _accountPublicKeyService.AddKey(accountId, request.PublicKey);
    }
    
    [Authorize]
    [HttpPost("revoke-key")]
    public async Task RevokeKey([FromBody] AccountRevokeKeyRequest request)
    {
        var accountId = _accountTokenService.GetIdentity(User).AccountId;
        await _accountPublicKeyService.RevokeKey(accountId, request.PublicKey);
    }
}

public class TestBody
{
    public string Message { get; set; }
    public string Signature { get; set; }
    public string PublicKey { get; set; }
}

public record AccountKeysResponse(string PublicKey, DateTime CreatedAtUtc, DateTime? RevokedAtUtc);

public record AccountRevokeKeyRequest
{
    public required string PublicKey { get; init; }
}

public record AccountAddKeyRequest
{
    public required string PublicKey { get; init; }
}
