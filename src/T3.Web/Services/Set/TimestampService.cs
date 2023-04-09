using MongoDB.Driver;
using T3.Web.Services.Set.Entities;
using T3.Web.Services.Set.Models;
using T3.Web.Services.Shared;

namespace T3.Web.Services.Set;

public interface ITimestampService
{
    Task<bool> IsValidTimestamp(ServerTimestamp timestamp);
    Task EnsureRecentTimestamp();
    Task<ServerTimestampEntity?> GetLatestOrDefault();
    Task<ServerTimestampEntity> GetLatest();
}

public class TimestampService : ITimestampService
{
    /// <summary>
    ///     The maximum number of hours a timestamp can be out of date before a new one is generated.
    /// </summary>
    private const int MaxHoursRecentTimestamp = 12;

    private static readonly object SyncLock = new();
    private readonly IMongoCollection<ServerTimestampEntity> _collection;
    private readonly IDateTimeService _dateTimeService;

    public TimestampService(
        IMongoCollection<ServerTimestampEntity> collection,
        IDateTimeService dateTimeService)
    {
        _collection = collection;
        _dateTimeService = dateTimeService;
    }

    public async Task<bool> IsValidTimestamp(ServerTimestamp timestamp)
    {
        var serverTimestampTask = _collection.Find(x =>
            x.Year == timestamp.Year &&
            x.DayOfYear == timestamp.DayOfYear &&
            x.MillisecondOfDay == timestamp.MillisecondOfDay &&
            x.Noise == timestamp.Noise
        ).SingleOrDefaultAsync();

        var serverTimestamp = await serverTimestampTask;
        return serverTimestamp != null;
    }

    public async Task EnsureRecentTimestamp()
    {
        try
        {
            Monitor.Enter(SyncLock);
            var latest = await GetLatestOrDefault();
            if (latest == null || RequireServerSideRefresh(latest))
                await InsertNew();
        }
        finally
        {
            Monitor.Exit(SyncLock);
        }
    }

    public async Task<ServerTimestampEntity> GetLatest()
    {
        return await QueryLatest().SingleAsync();
    }

    public async Task<ServerTimestampEntity?> GetLatestOrDefault()
    {
        return await QueryLatest().SingleOrDefaultAsync();
    }

    private IFindFluent<ServerTimestampEntity, ServerTimestampEntity> QueryLatest()
    {
        return _collection.Find(FilterDefinition<ServerTimestampEntity>.Empty)
            .SortByDescending(x => x.Year)
            .ThenByDescending(x => x.DayOfYear)
            .ThenByDescending(x => x.MillisecondOfDay)
            .Limit(1);
    }

    private bool RequireServerSideRefresh(ServerTimestamp latest)
    {
        var serverTime = ServerTimestamp.ToUtcDateTime(latest);
        var difference = _dateTimeService.UtcNow - serverTime;
        return difference.TotalHours > 12;
    }

    private async Task InsertNew()
    {
        var now = _dateTimeService.UtcNow;
        var newTimestamp = new ServerTimestampEntity
        {
            Id = Guid.NewGuid(),
            Year = now.Year,
            DayOfYear = now.DayOfYear,
            MillisecondOfDay = DateTime.UtcNow.Millisecond,
            Noise = Random.Shared.Next()
        };

        await _collection.InsertOneAsync(newTimestamp);
    }
}