using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using T3.Web.Services.Commit.Models;
using T3.Web.Services.Identity;
using T3.Web.Services.Set.ValueObjects;
using T3.Web.Services.Timestamp;

namespace T3.Web.Services.Commit;

public interface ISetCommitService
{
    Task<IEnumerable<SetCommit>> GetAll(SetId setId);
    Task Add(SetCommit commit);
}

public class SetCommitService : ISetCommitService
{
    private readonly IMongoCollection<SetCommit> _collection;
    private readonly ILogger<SetCommitService> _logger;
    private readonly IAccountPublicKeyService _accountPublicKeyService;
    private readonly ITimestampService _timestampService;

    public SetCommitService(
        ILogger<SetCommitService> logger,
        IMongoCollection<SetCommit> collection,
        IAccountPublicKeyService accountPublicKeyService, ITimestampService timestampService)
    {
        _logger = logger;
        _collection = collection;
        _accountPublicKeyService = accountPublicKeyService;
        _timestampService = timestampService;
    }

    public async Task<IEnumerable<SetCommit>> GetAll(SetId setId)
    {
        return await _collection.Find(x => x.Header.SetId.Value == setId.Value).ToListAsync();
    }

    public async Task Add(SetCommit commit)
    {
        ValidateSignature(commit);
        ValidatePayload(commit);
        await ValidateAuthor(commit);
        await ValidateTime(commit);

        await _collection.InsertOneAsync(commit);
    }

    private async Task ValidateTime(SetCommit commit)
    {
        var serverTimestamp = commit.Header.CreatedAt.ServerTimestamp;
        var isValid = await _timestampService.IsValidTimestamp(serverTimestamp);
        if (!isValid)
        {
            throw new Exception("Server timestamp is not valid")
            {
                Data =
                {
                    { "ServerTimestamp", serverTimestamp },
                    { "Commit", commit }
                }
            };
        }
    }

    private async Task ValidateAuthor(SetCommit commit)
    {
        var accountId = commit.Header.Author.UserId.Value;
        var publicKey = commit.Signature.PublicKey;
        var isValid = await _accountPublicKeyService.IsKeyValid(accountId, publicKey);
        if (!isValid)
        {
            throw new Exception("Public key is not associated with the author id");
        }
    }

    private void ValidatePayload(SetCommit commit)
    {
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters =
            {
                new JsonStringEnumConverter(),
                new SetCommitBodyConvertor()
            }
        };

        var commitElement = JsonSerializer.SerializeToElement(commit, jsonSerializerOptions);
        var verifyElement = JsonSerializer.Deserialize<JsonElement>(commit.Signature.Payload);

        // Get all the paths that are in the verify element
        var verifyValues = new Dictionary<string, object?>();
        var commitValues = new Dictionary<string, object?>();
        FillValuesDictionary(string.Empty, verifyValues, verifyElement);
        FillValuesDictionary(string.Empty, commitValues, commitElement);

        // Check if the commit element is a subset of the verify element
        var missingKeys = verifyValues.Keys.Except(commitValues.Keys).ToList();
        var invalidValues = verifyValues.Where(x => commitValues.ContainsKey(x.Key))
            .Select(verifyKp => new
            {
                path = verifyKp.Key,
                commitValue = commitValues[verifyKp.Key],
                proofValue = verifyKp.Value,
                areEqual = object.Equals(commitValues[verifyKp.Key], verifyKp.Value)
            })
            .Where(x => !x.areEqual)
            .ToList();


        if (missingKeys.Any() || invalidValues.Any())
        {
            using (var scope = _logger.BeginScope("Payload signature does not match the commit"))
            {
                foreach (var key in missingKeys)
                {
                    _logger.LogTrace("Missing key: {Key}", key);
                }

                foreach (var invalidValue in invalidValues)
                {
                    _logger.LogTrace("Invalid value: {Path}, in commit: {CommitValue}, in proof: {ProofValue}", invalidValue.path, invalidValue.commitValue, invalidValue.proofValue);
                }
            }

            throw new Exception("Payload signature does not match the commit")
            {
                Data =
                {
                    { nameof(missingKeys), missingKeys },
                    { nameof(invalidValues), invalidValues }
                }
            };
        }

        // Signature must proof:
        // - Author (Since afterwards we should be able to tell who the author is)
        // - CommitId (Since we should be able to tell if the commit is a duplicate)
        // - SetId (Since we should be able to tell if the commit is for the correct set)
        // - CreatedAt (Since we should be able to tell if the commit is too old)
        // The payload should provide more values.
        string[] requiredPaths = new[]
        {
            ".header.author.userId.value",
            ".header.commitId.value",
            ".header.setId.value",
            ".header.createdAt.serverTimestamp.year",
            ".header.createdAt.serverTimestamp.dayOfYear",
            ".header.createdAt.serverTimestamp.millisecondOfDay",
            ".header.createdAt.serverTimestamp.noise",
            ".header.createdAt.clientOffset.milliseconds",
        };

        var missingPaths = requiredPaths.Where(x => !verifyValues.ContainsKey(x));

        if (missingPaths.Any())
            throw new Exception("Payload signature must proof author id")
            {
                Data = { { nameof(missingPaths), missingPaths } }
            };
    }

    private void FillValuesDictionary(string path, Dictionary<string, object?> dictionary, JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Undefined:
                // Ignored
                break;
            case JsonValueKind.Object:
                var obj = element.EnumerateObject();
                foreach (var ar in obj) FillValuesDictionary(path + "." + ar.Name, dictionary, ar.Value);
                break;
            case JsonValueKind.Array:
                var array = element.EnumerateArray().ToArray();
                for (var i = 0; i < array.Length; i++)
                {
                    var ar = array[i];
                    FillValuesDictionary(path + "[" + i + "]", dictionary, ar);
                }

                break;
            case JsonValueKind.String:
                dictionary.Add(path, element.GetString());
                break;
            case JsonValueKind.Number:
                dictionary.Add(path, element.GetDouble());
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                dictionary.Add(path, element.GetBoolean());
                break;
            case JsonValueKind.Null:
                dictionary.Add(path, null);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ValidateSignature(SetCommit commit)
    {
        var jsonWebKey = JsonWebKey.Create(commit.Signature.PublicKey);
        // Verify the signature of the message that was sent
        var algorithm = CryptoProviderFactory.Default.CreateForVerifying(jsonWebKey, "RS256");
        var isValid = algorithm.Verify(Encoding.UTF8.GetBytes(commit.Signature.Payload),
            Convert.FromBase64String(commit.Signature.Signature));
        if (isValid) return;

        throw new Exception("Signature is not valid")
        {
            Data =
            {
                { "payload", commit.Signature.Payload },
                { "signature", commit.Signature.Signature },
                { "publicKey", commit.Signature.PublicKey }
            }
        };
    }
}