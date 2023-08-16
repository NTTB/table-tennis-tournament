using MongoDB.Driver;

namespace T3.Web.Services.Identity;

public record AccountCreateRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public record AccountCreateResponse
{
    public required string Username { get; set; }
    public required Guid Id { get; set; }
    public required DateTime CreatedAtUtc { get; set; }
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
            CreatedAtUtc = DateTime.UtcNow,
            Keys = Array.Empty<AccountKey>()
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