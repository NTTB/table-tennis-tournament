using System.Linq.Expressions;
using NSubstitute;
using Redis.OM.Searching;
using T3.Account.Api.Entities;
using T3.Account.Api.Repositories;

namespace T3.Account.Api.Test.Repositories;

public class AccountRepositoryTests
{
    [Test]
    public async Task FindByUsername__none_found()
    {
        var collection = Substitute.For<IRedisCollection<AccountEntity>>();
        var expressions = new List<Expression<Func<AccountEntity, bool>>>();
        collection.SingleOrDefaultAsync(Arg.Do<Expression<Func<AccountEntity, bool>>>(x => expressions.Add(x))).Returns(Task.FromResult<AccountEntity?>(null));
        var sut = new AccountRepository(collection);
        var user = await sut.FindByUsername("test");
        Assert.That(user, Is.Null);
        
        Assert.Multiple(() =>
        {
            Assert.That(expressions, Has.Count.EqualTo(1));
            Assert.That(expressions.First().Compile().Invoke(new AccountEntity { Username = "test", PasswordHash = null!}), Is.True);
        });
    }

    [Test]
    public async Task FindByUsername__returns_result()
    {
        var collection = Substitute.For<IRedisCollection<AccountEntity>>();
        Guid id = Guid.NewGuid();
        var fakeUser = new AccountEntity
        {
            Id = id,
            Username = "test",
            PasswordHash = "test"
        };

        var expressions = new List<Expression<Func<AccountEntity, bool>>>();
        collection.SingleOrDefaultAsync(Arg.Do<Expression<Func<AccountEntity, bool>>>(x => expressions.Add(x))).Returns(Task.FromResult<AccountEntity?>(fakeUser));
        var sut = new AccountRepository(collection);

        var user = await sut.FindByUsername("test");
        Assert.That(user, Is.EqualTo(fakeUser));
        Assert.Multiple(() =>
        {
            Assert.That(expressions, Has.Count.EqualTo(1));
            Assert.That(expressions.First().Compile().Invoke(new AccountEntity { Username = "test", PasswordHash = null!}), Is.True, "Username should match");
            Assert.That(expressions.First().Compile().Invoke(new AccountEntity { Username = "wrong", PasswordHash = null!}), Is.False, "Username should not match");
        });
    }   
    
    [Test]
    public async Task FindByGuid__returns_result()
    {
        var collection = Substitute.For<IRedisCollection<AccountEntity>>();
        Guid id = Guid.NewGuid();
        var fakeUser = new AccountEntity
        {
            Id = id,
            Username = "test",
            PasswordHash = "test"
        };
        
        var expressions = new List<Expression<Func<AccountEntity, bool>>>();
        collection.SingleOrDefaultAsync(Arg.Do<Expression<Func<AccountEntity, bool>>>(x => expressions.Add(x))).Returns(Task.FromResult<AccountEntity?>(fakeUser));
        var sut = new AccountRepository(collection);

        var user = await sut.FindByGuid(id);
        Assert.That(user, Is.EqualTo(fakeUser));
        
        Assert.Multiple(() =>
        {
            Assert.That(expressions, Has.Count.EqualTo(1));
            Assert.That(expressions.First().Compile().Invoke(new AccountEntity { Id = id, PasswordHash = null!}), Is.True, "Id should match");
            Assert.That(expressions.First().Compile().Invoke(new AccountEntity { Id = Guid.NewGuid(), PasswordHash = null!}), Is.False, "Id should not match");
        });
    }
}