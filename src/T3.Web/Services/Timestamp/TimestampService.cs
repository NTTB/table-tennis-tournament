using MongoDB.Driver;
using T3.Web.Services.Shared;
using T3.Web.Services.Timestamp.Entities;
using T3.Web.Services.Timestamp.Models;

namespace T3.Web.Services.Timestamp;

public interface ITimestampService
{
    Task<bool> IsValidTimestamp(ServerTimestamp timestamp);
    Task<ServerTimestampEntity> CreateNew();
}

public class TimestampService : ITimestampService
{
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
    
    public async Task<ServerTimestampEntity> CreateNew()
    {
        var now = _dateTimeService.UtcNow;
        var newTimestamp = new ServerTimestampEntity
        {
            Id = Guid.NewGuid(),
            Year = now.Year,
            DayOfYear = now.DayOfYear,
            MillisecondOfDay = (int)now.TimeOfDay.TotalMilliseconds,
            Noise = Random.Shared.Next()
        };

        await _collection.InsertOneAsync(newTimestamp);
        return newTimestamp;
    }
}