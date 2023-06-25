using MongoDB.Driver;
using T3.Web.Services.Identity.Entities;

namespace T3.Web.Services.Identity;

public record AccountLoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public record AccountLoginResponse
{
    public string JwtToken { get; init; }
}

public interface IAccountLoginService
{
    Task<AccountLoginResponse> Login(AccountLoginRequest request);
}

public class AccountLoginService : IAccountLoginService
{
    private readonly IMongoCollection<AccountEntity> _accountCollection;
    private readonly IPasswordService _passwordService;
    private readonly IAccountTokenService _tokenService;

    public AccountLoginService(
        IMongoCollection<AccountEntity> accountCollection, 
        IPasswordService passwordService,
        IAccountTokenService tokenService)
    {
        _accountCollection = accountCollection;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<AccountLoginResponse> Login(AccountLoginRequest request)
    {
        // Find the user
        var user = await _accountCollection.Find(x => x.Username == request.Username).SingleOrDefaultAsync();
        if (user == null)
            throw new Exception("User not found");

        // Check the password
        var passwordValue = PasswordValue.Parse(user.PasswordHash);
        var isValidPassword = _passwordService.Verify(request.Password, passwordValue);
        if (!isValidPassword)
            throw new Exception("Invalid password");

        var token = await _tokenService.GenerateJwtToken(user);
        return new AccountLoginResponse()
        {
            JwtToken = token
        };
    }
}