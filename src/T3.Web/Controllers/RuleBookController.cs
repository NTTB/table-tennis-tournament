using Microsoft.AspNetCore.Mvc;
using T3.Web.Services.Rules;
using T3.Web.Services.Rules.Models;
using T3.Web.Services.Rules.ValueObjects;

namespace T3.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RuleBookController : ControllerBase
{
    private readonly IRulebookService _rulebookService;

    public RuleBookController(IRulebookService rulebookService)
    {
        _rulebookService = rulebookService;
    }

    [HttpGet("{id}")]
    public RuleBookModel GetRuleBook(RulebookId id)
    {
        return _rulebookService.GetById(id);
    }
    
    [HttpGet]
    public RulebookListItem[] GetRuleBookList()
    {
        return _rulebookService.GetOverviewList();
    }
}