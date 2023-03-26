using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TTT.Web.Services.Identity.Models;

namespace TTT.Web.Services.Identity;

public interface IAccountTokenService
{
    Task<string> GenerateJwtToken(AccountEntity user);
}

public class AccountTokenService : IAccountTokenService
{
    private readonly IOptions<Settings> _settings;

    public class Settings
    {
        public string SecretKey { get; set; }
    }

    public AccountTokenService(IOptions<Settings> settings)
    {
        _settings = settings;
    }
    
    public async Task<string> GenerateJwtToken(AccountEntity user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.SecretKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                // new Claim(ClaimTypes.Email, "john.doe@example.com")
            }),
            Expires = DateTime.UtcNow.AddMonths(6),
            SigningCredentials = signingCredentials,
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);
        return token;
    }
}