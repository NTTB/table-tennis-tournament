using System.Security.Cryptography;
using JWT.Algorithms;
using T3.Account.Api.Errors;
using T3.Account.Api.Settings;

namespace T3.Account.Api.Services;

public interface IWebTokenAlgorithmGenerator : IDisposable
{
    IJwtAlgorithm Algorithm { get; }

    public Task LoadFromDisk();
}

public sealed class WebTokenAlgorithmGenerator : IWebTokenAlgorithmGenerator
{
    private readonly WebTokenSettings _settings;
    private RSA? _publicKey;
    private RSA? _privateKey;

    public WebTokenAlgorithmGenerator(WebTokenSettings settings)
    {
        _settings = settings;
    }

    public IJwtAlgorithm Algorithm { get; private set; } = new NoneAlgorithm();

    public async Task LoadFromDisk()
    {
        var publicKeyExistsOnDisk = File.Exists(_settings.PublicKeyPath);
        var privateKeyExistsOnDisk = File.Exists(_settings.PrivateKeyPath);
        if (publicKeyExistsOnDisk && !privateKeyExistsOnDisk)
            throw new WebTokenGeneratorException("Public key exists but private key is missing");
        if (privateKeyExistsOnDisk && !publicKeyExistsOnDisk)
            throw new WebTokenGeneratorException("Private key exists but public key is missing");

        if (!publicKeyExistsOnDisk && !privateKeyExistsOnDisk)
            await CreateKeyOnDisk();

        // Load file
        var publicKeyBytesTask = File.ReadAllTextAsync(_settings.PublicKeyPath);
        var privateKeyBytesTask = File.ReadAllTextAsync(_settings.PrivateKeyPath);

        var oldPublicKey = _publicKey;
        var oldPrivateKey = _privateKey;

        var newPublicKey = RSA.Create(2048);
        var newPrivateKey = RSA.Create(2048);

        try
        {
            newPublicKey.ImportFromPem(await publicKeyBytesTask);
        }
        catch
        {
            newPublicKey.Dispose();
            newPrivateKey.Dispose();
            throw new WebTokenGeneratorException("Failed to load public key");
        }

        try
        {
            newPrivateKey.ImportFromPem(await privateKeyBytesTask);
        }
        catch
        {
            newPublicKey.Dispose();
            newPrivateKey.Dispose();
            throw new WebTokenGeneratorException("Failed to load private key");
        }
        
        _publicKey = newPublicKey;
        _privateKey = newPrivateKey;
        Algorithm = new RS2048Algorithm(_publicKey, _privateKey);
        
        oldPublicKey?.Dispose();
        oldPrivateKey?.Dispose();

    }

    private async Task CreateKeyOnDisk()
    {
        using var rsa = RSA.Create(2048);
        var publicKey = rsa.ExportRSAPublicKeyPem();
        var privateKey = rsa.ExportRSAPrivateKeyPem();
        await File.WriteAllTextAsync(_settings.PublicKeyPath, publicKey);
        await File.WriteAllTextAsync(_settings.PrivateKeyPath, privateKey);
    }

    public void Dispose()
    {
        _publicKey?.Dispose();
        _privateKey?.Dispose();
    }
}