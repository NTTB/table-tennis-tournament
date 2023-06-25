using Microsoft.AspNetCore.Mvc;
using T3.Web.Services.Set;
using T3.Web.Services.Set.Entities;

namespace T3.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetController : ControllerBase
{
    private readonly ISetService _setService;

    public SetController(
        ISetService setService
    )
    {
        _setService = setService;
    }
    
    [HttpGet("")]
    public async Task<SetEntity[]> Search()
    {
        return await _setService.GetAll();
    }
    
    [HttpGet("{id}")]
    public async Task<SetEntity> GetById(Guid id)
    {
        return await _setService.GetById(id);
    }

    [HttpDelete("{id}")]
    public async Task Delete(Guid id)
    {
        await _setService.DeleteById(id);
    }

    [HttpPost("create")]
    public async Task<SetEntity> Create([FromBody] CreateSetRequest request)
    {
        var result = await _setService.CreateSet(request);
        return result.Entity;
    }
}