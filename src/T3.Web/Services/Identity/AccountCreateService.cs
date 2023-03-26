using MongoDB.Driver;
using T3.Web.Services.Identity.Models;

namespace T3.Web.Services.Identity;

public record AccountCreateRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public record AccountCreateResponse
{
    public string Username { get; set; }
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public interface IAccountCreateService
{
    Task<AccountCreateResponse> CreateNew(AccountCreateRequest request);
}

public class AccountCreateService : IAccountCreateService
{
    private readonly IMongoCollection<AccountEntity> _collection;
    private readonly IPasswordService _passwordService;

    public AccountCreateService(
        IMongoCollection<AccountEntity> collection,
        IPasswordService passwordService)
    {
        _collection = collection;
        _passwordService = passwordService;
    }

    public async Task<AccountCreateResponse> CreateNew(AccountCreateRequest request)
    {
        await ThrowIfUsernameIsUsed(request.Username);

        var password = _passwordService.Hash(request.Password);
        var user = new AccountEntity()
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = password.ToSerializedString(),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(user);

        return new AccountCreateResponse()
        {
            Id = user.Id,
            Username = user.Username,
            CreatedAtUtc = user.CreatedAtUtc
        };
    }

    private async Task ThrowIfUsernameIsUsed(string username)
    {
        bool usernameInUse = await _collection.Find(x => x.Username == username).AnyAsync();
        if (usernameInUse)
        {
            throw new Exception("Username already in use")
            {
                Data = { { "Username", username } }
            };
        }
    }
}