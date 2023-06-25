using MongoDB.Driver;
using T3.Web.Services.Data.Migrations;
using T3.Web.Services.Rules;
using T3.Web.Services.Rules.Entities;

namespace T3.Web.Migrations;

public class Migration_20230528_2041_AddNttbRulebook : IMigration
{
    private readonly IMongoCollection<RuleBookEntity> _collection;

    public Migration_20230528_2041_AddNttbRulebook(IMongoCollection<RuleBookEntity> collection)
    {
        _collection = collection;
    }

    public async Task Up()
    {
        var rulebook = new RuleBookEntity
        {
            Id = RulebookIds.NttbRulebookId2023.Value,
            DisplayName = "NTTB Rule Book 2023",
            OffenseTypes = new[]
            {
                new OffenseType("01", "schoppen tegen de tafel of de omheining etc.", false),
                new OffenseType("02", "gooien met het batje", false),
                new OffenseType("03", "vloeken, schelden of het uiten van onwelvoeglijke taal", false),
                new OffenseType("05", "wegschoppen of wegslaan van de bal", false),
                new OffenseType("07",
                    "niet op tijd terugkeren van de pauze tussen twee games of een time out (na waarschuwing)", false),
                new OffenseType("08", "coachen tijdens het spel in woord en/of gebaar", false),
                new OffenseType("10", "met vuist of bat op de tafel slaan", false),
                new OffenseType("12", "het moedwillig verplaatsen of verbouwen van de afzetting", false),
                new OffenseType("18", "met het bat op de grond of ander object slaan", false),
                new OffenseType("20", "overige (nader te omschrijven)", true)
            }
        };

        // If missing insert the rulebook
        if(!await _collection.Find(x => x.Id == rulebook.Id).AnyAsync())
            await _collection.InsertOneAsync(rulebook);
    }
}