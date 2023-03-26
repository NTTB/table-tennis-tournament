using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace TTT.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class SystemController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMongoClient _client;

    public SystemController(ILogger<WeatherForecastController> logger, IMongoClient client)
    {   
        _logger = logger;
        _client = client;
    }

    [HttpGet("debug/databases")]
    public IEnumerable<string> DebugGetDatabases()
    {
        var database =_client.ListDatabaseNames();
        return database.ToList();
    }
}