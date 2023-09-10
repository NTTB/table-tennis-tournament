using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using T3.Account.Api.Values;

namespace T3.Account.Api.Services;

public interface IPasswordService
{
    public PasswordValue Hash(string password);
    public bool Verify(string password, PasswordValue hash);
}

// NOTE: There is a reasonable chance the the password service might change in the future,
// so we need to be able to support multiple versions.
public class PasswordV1Service : IPasswordService
{
    private readonly IOptions<Settings> _settings;
    public const string Version = "v1";

    public class Settings
    {
        public int SaltSize { get; set; }
        public int HashSize { get; set; }
        public int Iterations { get; set; }
    }

    public PasswordV1Service(IOptions<Settings> settings)
    {
        _settings = settings;
    }

    public PasswordValue Hash(string password)
    {
        var iterations = _settings.Value.Iterations;
        
        byte[] salt = GenerateSalt(_settings.Value.SaltSize);
        byte[] hash = GenerateHash(password, salt, iterations, _settings.Value.HashSize);

        return new PasswordValue()
        {
            Version = Version,
            Iterations = iterations,
            Hash = hash,
            Salt = salt
        };
    }

    private byte[] GenerateSalt(int saltLength)
    {
        return RandomNumberGenerator.GetBytes(saltLength);
    }

    private byte[] GenerateHash(string password, byte[] salt, int iterations, int hashLength)
    {
        using var hash = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512);
        return hash.GetBytes(hashLength);
    }

    public bool Verify(string password, PasswordValue hashedPassword)
    {
        var hashedInput = GenerateHash(password, hashedPassword.Salt, hashedPassword.Iterations, hashedPassword.Hash.Length);
        return SlowEquals(hashedPassword.Hash, hashedInput);
    }
    
    // Compares two byte arrays in length-constant time.
    // This comparison method is used so that password hashes cannot be extracted from on-line systems using a timing attack and then attacked off-line.
    private static bool SlowEquals(byte[] a, byte[] b) {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++) {
            diff |= (uint)(a[i] ^ b[i]);
        }
        return diff == 0;
    }
}