using Microsoft.AspNetCore.Mvc;
using T3.Web.Services.Commit.ValueObjects;
using T3.Web.Services.Set;
using T3.Web.Services.Set.Entities;
using T3.Web.Services.SetValidation;
using T3.Web.Services.SetValidation.Models;

namespace T3.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetController : ControllerBase
{
    private readonly ISetService _setService;
    private readonly ISetCommitValidateService _setCommitValidateService;

    public SetController(
        ISetService setService,
        ISetCommitValidateService setCommitValidateService
    )
    {
        _setService = setService;
        _setCommitValidateService = setCommitValidateService;
    }
    
    [HttpGet("")]
    public async Task<SetEntity[]> Search()
    {
        return await _setService.GetAll();
    }
    
    [HttpGet("{id:guid}")]
    public async Task<SetEntity> GetById(Guid id)
    {
        return await _setService.GetById(id);
    }

    [HttpDelete("{id:guid}")]
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
    
    [HttpPost("{id:guid}/commits/{commitId:guid}/validate")]
    public async Task<SetCommitValidationResult> Validate(Guid id, Guid commitId)
    {
        return await _setCommitValidateService.ValidateCommit(new CommitId(commitId));
    }
}