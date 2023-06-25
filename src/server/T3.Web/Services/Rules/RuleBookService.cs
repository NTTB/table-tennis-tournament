using MongoDB.Driver;
using T3.Web.Services.Rules.Entities;
using T3.Web.Services.Rules.Models;
using T3.Web.Services.Rules.ValueObjects;

namespace T3.Web.Services.Rules;

public interface IRulebookService
{
    RuleBookModel GetById(RulebookId rulebookId);
    RulebookListItem[] GetOverviewList();
}

public class RulebookService : IRulebookService
{
    private IMongoCollection<RuleBookEntity> _collection;

    public RulebookService(IMongoCollection<RuleBookEntity> collection)
    {
        _collection = collection;
    }

    public RuleBookModel GetById(RulebookId rulebookId)
    {
        var entity = _collection.Find(x => x.Id == rulebookId.Value).Single();
        return new RuleBookModel()
        {
            Id = new RulebookId() { Value = entity.Id },
            DisplayName = entity.DisplayName,
            OffenseTypes = entity.OffenseTypes
        };
    }

    public RulebookListItem[] GetOverviewList()
    {
        return _collection.Find(x => true)
            .ToList()
            .Select(x => new RulebookListItem(new RulebookId(){Value = x.Id}, x.DisplayName))
            .ToArray();
    }
}