using MongoDB.Driver;
using T3.Web.Services.SecretNotes.Entities;
using T3.Web.Services.SecretNotes.ValueObjects;

namespace T3.Web.Services.SecretNotes;

public interface ISecretNoteService
{
    Task<SecretNote> CreateAsync(SecretNote secretNote);
    Task<List<SecretNote>> FindAsync(SecretNoteId id);
    Task<SecretNote?> GetAsync(SecretNoteId id, SecretNoteVersionId versionId);
}

public class SecretNoteService : ISecretNoteService
{
    private readonly IMongoCollection<SecretNote> _collection;

    public SecretNoteService(IMongoCollection<SecretNote> collection)
    {
        _collection = collection;
    }

    public async Task<SecretNote> CreateAsync(SecretNote secretNote)
    {
        await _collection.InsertOneAsync(secretNote);
        return secretNote;
    }

    public async Task<List<SecretNote>> FindAsync(SecretNoteId id)
    {
        var filter = Builders<SecretNote>.Filter.Eq(x => x.Content.Id, id);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<SecretNote?> GetAsync(SecretNoteId id, SecretNoteVersionId versionId)
    {
        var filter = Builders<SecretNote>.Filter.And(
            Builders<SecretNote>.Filter.Eq(x => x.Content.Id, id),
            Builders<SecretNote>.Filter.Eq(x => x.Content.VersionId, versionId)
        );

        return await _collection.Find(filter).SingleOrDefaultAsync();
    }
}