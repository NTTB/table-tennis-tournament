using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using T3.Web.Services.Timestamp;
using T3.Web.Services.Timestamp.Models;

namespace T3.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TimestampController
{
    private readonly ITimestampService _timestampService;

    public TimestampController(ITimestampService timestampService)
    {
        _timestampService = timestampService;
    }
    
    [HttpPost("latest")]
    public async Task<ServerTimestamp> CreateAndGetLatest()
    {
        var latest = await _timestampService.CreateNew();
        return new ServerTimestamp
        {
            Year = latest.Year,
            DayOfYear = latest.DayOfYear,
            MillisecondOfDay = latest.MillisecondOfDay,
            Noise = latest.Noise,
        };
    }
}