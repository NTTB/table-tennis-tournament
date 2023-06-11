using Microsoft.AspNetCore.Mvc;
using T3.Web.Services.Identity;
using T3.Web.Services.SecretNotes;
using T3.Web.Services.SecretNotes.Entities;
using T3.Web.Services.SecretNotes.ValueObjects;

namespace T3.Web.Controllers;

/// <summary>
/// Since notes are not shared realtime, we use a controller.
/// The calling client should however only attach a note when creation was confirmed by this controller.
/// </summary>
[ApiController]
[Route("api/secret-notes")]
public class SecretNoteController : ControllerBase
{
    private readonly ISecretNoteService _secretNoteService;
    private readonly IAccountTokenService _accountTokenService;

    public SecretNoteController(
        ISecretNoteService secretNoteService,
        IAccountTokenService accountTokenService)
    {
        _secretNoteService = secretNoteService;
        _accountTokenService = accountTokenService;
    }
    
    [HttpGet("{id}")]
    public async Task<List<SecretNote>> FindAsync(SecretNoteId id)
    {
        return await _secretNoteService.FindAsync(id);
    }
    
    [HttpGet("{id}/{versionId}")]
    public async Task<SecretNote?> FindSpecificAsync(SecretNoteId id, SecretNoteVersionId versionId)
    {
        return await _secretNoteService.GetAsync(id, versionId);
    }
    
    [HttpPost]
    public async Task<SecretNote> CreateAsync([FromBody]SecretNote secretNote)
    {
        return await _secretNoteService.CreateAsync(secretNote);
    }
}