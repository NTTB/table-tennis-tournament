using Microsoft.AspNetCore.Mvc;
using T3.Web.Services.Commit;
using T3.Web.Services.Commit.Models;
using T3.Web.Services.Set;
using T3.Web.Services.Set.Entities;
using T3.Web.Services.Set.ValueObjects;

namespace T3.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetController : ControllerBase
{
    private readonly ISetService _setService;
    private readonly ISetCommitService _setCommitService;

    public SetController(
        ISetService setService,
        ISetCommitService setCommitService
    )
    {
        _setService = setService;
        _setCommitService = setCommitService;
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
    
    [HttpGet("{id}/commits")]
    public async Task<IEnumerable<SetCommit>> GetCommits(Guid id)
    {
        // NOTE: This function is not intended to be used by application for polling, so we will likely remove it.
        return await _setCommitService.GetAll(new SetId(id));
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