using Microsoft.AspNetCore.Mvc;
using T3.Web.Services.Set;
using T3.Web.Services.Set.Models;

namespace T3.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetController : ControllerBase
{
    private readonly ISearchSetService _searchSetService;
    private readonly IDeleteSetService _deleteSetService;
    private readonly IUpdateSetService _updateSetService;
    private readonly ICreateSetService _createSetService;

    public SetController(
        ISearchSetService searchSetService,
        IDeleteSetService deleteSetService,
        IUpdateSetService updateSetService,
        ICreateSetService createSetService
    )
    {
        _searchSetService = searchSetService;
        _deleteSetService = deleteSetService;
        _updateSetService = updateSetService;
        _createSetService = createSetService;
    }
    
    [HttpGet("")]
    public async Task<SetEntity[]> Search()
    {
        return await _searchSetService.GetAll();
    }
    
    [HttpGet("{id}")]
    public async Task<SetEntity> GetById(Guid id)
    {
        return await _searchSetService.GetById(id);
    }
    
    [HttpDelete("{id}")]
    public async Task Delete(Guid id)
    {
        await _deleteSetService.DeleteById(id);
    }

    [HttpPost("create")]
    public async Task<CreateSetResponse> Create([FromBody] CreateSetRequest request)
    {
        return await _createSetService.CreateSet(request);
    }
}