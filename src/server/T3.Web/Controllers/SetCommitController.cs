using Microsoft.AspNetCore.Mvc;
using T3.Web.Services.Commit.ValueObjects;
using T3.Web.Services.SetValidation;
using T3.Web.Services.SetValidation.Models;

namespace T3.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetCommitController : ControllerBase
{
    private readonly ISetCommitValidateService _setCommitValidateService;

    public SetCommitController(ISetCommitValidateService setCommitValidateService)
    {
        _setCommitValidateService = setCommitValidateService;
    }

    [HttpGet("{id}/validate")]
    public async Task<SetCommitValidationResult> Validate(Guid id)
    {
        return await _setCommitValidateService.ValidateCommit(new CommitId(id));
    }
}