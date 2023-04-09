using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using T3.Web.Services.Set;
using T3.Web.Services.Set.Models;

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
        await _timestampService.EnsureRecentTimestamp();
        var latest = await _timestampService.GetLatest();
        return new ServerTimestamp
        {
            Year = latest.Year,
            DayOfYear = latest.DayOfYear,
            MillisecondOfDay = latest.MillisecondOfDay,
            Noise = latest.Noise,
        };
    }
}