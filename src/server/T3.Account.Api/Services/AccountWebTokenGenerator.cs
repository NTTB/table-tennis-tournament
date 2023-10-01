using System.Security.Cryptography;
using JWT.Algorithms;
using JWT.Builder;
using T3.Account.Api.Entities;

namespace T3.Account.Api.Services;

public record AccountInfo(string Subject, string GivenName);

public interface IAccountWebTokenGenerator
{
    string Generate(AccountInfo entity, string audience);
}

public class AccountWebTokenGenerator : IAccountWebTokenGenerator
{
    private readonly IWebTokenAlgorithmGenerator _algorithmGenerator;

    public AccountWebTokenGenerator(IWebTokenAlgorithmGenerator algorithmGenerator)
    {
        _algorithmGenerator = algorithmGenerator;
    }

    public string Generate(AccountInfo entity, string audience)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity), "AccountEntity cannot be null");
        var token = JwtBuilder.Create()
                .Subject(entity.Subject)
                .GivenName(entity.GivenName)
                .Audience(audience) // Must be unique (comes from config)
                .Issuer("issuer") // Must be unique (comes from config)
                .ExpirationTime(DateTime.Now.AddDays(21))
                .NotBefore(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                .IssuedAt(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                .WithAlgorithm(_algorithmGenerator.Algorithm)
            ;

        return token.Encode();
    }
}