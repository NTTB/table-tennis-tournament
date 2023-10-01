using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using Redis.OM.Searching;
using T3.Account.Api.Repositories;

namespace T3.Account.Api.Test.Repositories;

/// <summary>
///     To avoid having to create test for basic CRUD operations for each repository, we create a FakeRepository that
///     inherits from BaseRepository and adds nothing
/// </summary>
public class BaseRepositoryTests
{
    private IRedisCollection<FakeEntity> _collection = Substitute.For<IRedisCollection<FakeEntity>>();
    private IRepository<FakeEntity> _sut = Substitute.For<IRepository<FakeEntity>>();

    [ExcludeFromCodeCoverage(Justification = "Fake entity used for testing")]
    public class FakeEntity
    {
        public Guid Id { get; set; }
    }

    // ReSharper disable once MemberCanBePrivate.Global since it's used in the test
    public class FakeRepository : BaseRepository<FakeEntity>
    {
        public FakeRepository(IRedisCollection<FakeEntity> collection) : base(collection)
        {
        }
    }

    [SetUp]
    public void SetUp()
    {
        _collection = Substitute.For<IRedisCollection<FakeEntity>>();
        _sut = new FakeRepository(_collection);
    }

    [Test]
    public async Task InsertOne()
    {
        var id = Guid.NewGuid();
        var fakeUser = new FakeEntity
        {
            Id = id
        };

        _collection.InsertAsync(Arg.Any<FakeEntity>()).Returns(Task.FromResult<string>("fake-key"));
        await _sut.InsertOne(fakeUser);

        await _collection.Received(1).InsertAsync(Arg.Is<FakeEntity>(x => x == fakeUser));
    }

    [Test]
    public async Task UpdateOne()
    {
        var id = Guid.NewGuid();
        var fakeEntity = new FakeEntity { Id = id };

        _collection.UpdateAsync(Arg.Any<FakeEntity>()).Returns(Task.CompletedTask);
        await _sut.UpdateOne(fakeEntity);
        await _collection.Received(1).UpdateAsync(Arg.Is<FakeEntity>(x => x == fakeEntity));
    }
    
    [Test]
    public async Task DeleteOne()
    {
        var id = Guid.NewGuid();
        var fakeEntity = new FakeEntity { Id = id };

        _collection.DeleteAsync(Arg.Any<FakeEntity>()).Returns(Task.CompletedTask);
        await _sut.DeleteOne(fakeEntity);
        await _collection.Received(1).DeleteAsync(Arg.Is<FakeEntity>(x => x == fakeEntity));
    }
}