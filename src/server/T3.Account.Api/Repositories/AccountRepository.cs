using Redis.OM.Searching;
using T3.Account.Api.Entities;

namespace T3.Account.Api.Repositories;

public interface IAccountRepository : IRepository<AccountEntity>
{
    Task<AccountEntity?> FindByUsername(string username);
    Task<AccountEntity?> FindByGuid(Guid accountId);
}

public class AccountRepository : BaseRepository<AccountEntity>, IAccountRepository
{
    public AccountRepository(IRedisCollection<AccountEntity> collection) : base(collection)
    {
    }

    public async Task<AccountEntity?> FindByUsername(string username)
    {
        return await Collection.SingleOrDefaultAsync(x => x.Username == username);
    }

    public async Task<AccountEntity?> FindByGuid(Guid accountId)
    {
        return await Collection.SingleOrDefaultAsync(x => x.Id == accountId);
    }
}