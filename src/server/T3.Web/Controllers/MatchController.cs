using Microsoft.AspNetCore.Mvc;
using T3.Web.Services.Commit;
using T3.Web.Services.Match;
using T3.Web.Services.MatchValidation;

namespace T3.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchController : ControllerBase
{
    private readonly IMatchService _matchService;
    private readonly IMatchValidateService _matchValidateService;

    public MatchController(
        IMatchService matchService,
        IMatchValidateService matchValidateService
    )
    {
        _matchService = matchService;
        _matchValidateService = matchValidateService;
    }
    
    [HttpGet("")]
    public async Task<MatchEntity[]> Search()
    {
        return await _matchService.GetAll();
    }
    
    
    [HttpGet("{id:Guid}/validate")]
    public async Task<MatchValidationResult> Validate(Guid id)
    {
        return await _matchValidateService.ValidateCommit(new CommitId(id));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<MatchEntity> GetById(Guid id)
    {
        return await _matchService.GetById(id);
    }

    [HttpDelete("{id:guid}")]
    public async Task Delete(Guid id)
    {
        await _matchService.DeleteById(id);
    }

    [HttpPost("create")]
    public async Task<MatchEntity> Create([FromBody] CreateMatchRequest request)
    {
        var result = await _matchService.Create(request);
        return result.Entity;
    }
    
    [HttpPost("{id:guid}/commits/{commitId:guid}/validate")]
    public async Task<MatchValidationResult> Validate(Guid id, Guid commitId)
    {
        return await _matchValidateService.ValidateCommit(new CommitId(commitId));
    }
}