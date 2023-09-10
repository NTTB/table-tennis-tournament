using MongoDB.Driver;
using NRedisStack.RedisStackCommands;
using Redis.OM.Searching;
using T3.Account.Api.Entities;

namespace T3.Account.Api.Repositories;

public interface IRepository<T>
{
    Task InsertOne(T entity);
}

public interface IAccountRepository : IRepository<AccountEntity>
{
    Task<AccountEntity?> FindByUsername(string username);
}

public class AccountRepository : IAccountRepository
{
    private readonly IRedisCollection<AccountEntity> _accountCollection;

    public AccountRepository(IRedisCollection<AccountEntity> accountCollection)
    {
        _accountCollection = accountCollection;
    }

    public async Task InsertOne(AccountEntity entity)
    {
        await _accountCollection.InsertAsync(entity);
    }

    public async Task<AccountEntity?> FindByUsername(string username)
    {
        return await _accountCollection.SingleOrDefaultAsync(x=>x.Username == username);
    }


}