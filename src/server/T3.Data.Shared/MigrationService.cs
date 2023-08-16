using System.Text.RegularExpressions;
using MongoDB.Driver;

namespace T3.Data.Shared;

public interface IMigrationService
{
    public Task PerformMigration();
}

public class MigrationService : IMigrationService
{
    private readonly IEnumerable<IMigration> _migrations;
    private readonly IMongoCollection<MigrationEntity> _collection;

    public MigrationService(IEnumerable<IMigration> migrations, IMongoCollection<MigrationEntity> collection)
    {
        _migrations = migrations;
        _collection = collection;
    }
    
    public async Task PerformMigration()
    {
        // Verify that all migrations have the same naming scheme, this is to prevent ordering issues.
        // Name is: Migration_{yyyyMMdd}_{hhmm}_{name}
        var migrations = _migrations
            .Select(migration => new { Name= migration.GetType().Name,  migration})
            .OrderBy(x=>x.Name)
            .ToArray();
        
        var regex = new Regex(@"^Migration_(\d{8})_(\d{4})_(.*)$");
        foreach (var entry in migrations)
        {
            var match = regex.Match(entry.Name);
            if (!match.Success)
                throw new Exception($"Migration name {entry} does not match the required pattern.");
            
            // Check if the migration name was already performed
            bool canSkip = await _collection.Find(x => x.Name == entry.Name).AnyAsync();
            if(canSkip) continue;
            
            // Perform the migration
            try
            {
                await entry.migration.Up();
            }
            catch (Exception e)
            {
                throw new Exception($"Migration {entry.Name} failed.", e);
            }
            
            // Insert the migration into the database
            await _collection.InsertOneAsync(new MigrationEntity
            {
                Id = Guid.NewGuid(),
                Name = entry.Name,
                ExecutedAt = DateTime.UtcNow,
            });
        }
    }
}