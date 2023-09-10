using System.Linq.Expressions;
using NSubstitute;
using NSubstitute.Extensions;
using Redis.OM.Searching;
using T3.Account.Api.Entities;
using T3.Account.Api.Repositories;

namespace T3.Account.Api.Test.Repositories;

public class AccountRepositoryTests
{
    [Test]
    public async Task FindNone()
    {
        var collection = Substitute.For<IRedisCollection<AccountEntity>>();
        var sut = new AccountRepository(collection);
        var user = await sut.FindByUsername("test");
        Assert.That(user, Is.Null);
    } 
    [Test]
    public async Task FindOne()
    {
        var collection = Substitute.For<IRedisCollection<AccountEntity>>();
        var fakeUser = new AccountEntity(Guid.NewGuid(), "test", "test");

        collection.Configure().FirstOrDefaultAsync(Arg.Any<Expression<Func<AccountEntity, bool>>>()).Returns(Task.FromResult(fakeUser));
        var sut = new AccountRepository(collection);
        
        var user = await sut.FindByUsername("test");
        Assert.That(user, Is.EqualTo(fakeUser));
    } 
}