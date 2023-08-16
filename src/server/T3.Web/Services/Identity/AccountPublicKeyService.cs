using MongoDB.Driver;
using MongoDB.Driver.Linq;
using T3.Web.Services.Shared;

namespace T3.Web.Services.Identity;

public interface IAccountPublicKeyService
{
    Task AddKey(Guid accountId, string publicKey);
    Task RevokeKey(Guid accountId, string publicKey);
    Task<IEnumerable<AccountKey>> GetKeys(Guid accountId);

    Task<bool> IsKeyValid(Guid accountId, string publicKey);
}

public class AccountPublicKeyService : IAccountPublicKeyService
{
    private readonly IMongoCollection<AccountEntity> _accounts;
    private readonly IDateTimeService _dateTimeService;

    public AccountPublicKeyService(IMongoCollection<AccountEntity> accounts, IDateTimeService dateTimeService)
    {
        _accounts = accounts;
        _dateTimeService = dateTimeService;
    }

    public async Task AddKey(Guid accountId, string publicKey)
    {
        var alreadySet = await _accounts
            .Find(x => x.Id == accountId && x.Keys.Any(y => y.PublicKey == publicKey))
            .AnyAsync();

        if (alreadySet)
            throw new InvalidOperationException("Key already set (revoked keys can't be updated");

        await _accounts.FindOneAndUpdateAsync(x => x.Id == accountId,
            Builders<AccountEntity>.Update.Push(x => x.Keys, new AccountKey
            {
                CreatedAtUtc = _dateTimeService.UtcNow,
                RevokedAtUtc = null,
                PublicKey = publicKey
            }));
    }

    public async Task RevokeKey(Guid accountId, string publicKey)
    {
        // Search for the account in the collection, and then of that account find the element in the
        // keys array so that we can set the revoked at date value 

        var filter = Builders<AccountEntity>.Filter.And(
            Builders<AccountEntity>.Filter.Eq(x => x.Id, accountId),
            Builders<AccountEntity>.Filter.ElemMatch(x => x.Keys, y => y.PublicKey == publicKey)
        );
        
        var update = Builders<AccountEntity>.Update.Set(
            x => x.Keys.FirstMatchingElement().RevokedAtUtc, 
            _dateTimeService.UtcNow
            );
        await _accounts.FindOneAndUpdateAsync(
            filter,
            update
        );
    }

    public async Task<IEnumerable<AccountKey>> GetKeys(Guid accountId)
    {
        return await _accounts
            .Find(x => x.Id == accountId)
            .Project(x => x.Keys)
            .SingleAsync();
    }
    
    public async Task<bool> IsKeyValid(Guid accountId, string publicKey)
    {
        return await _accounts
            .Find(x => x.Id == accountId && x.Keys.Any(y => y.PublicKey == publicKey && y.RevokedAtUtc == null))
            .AnyAsync();
    }
}