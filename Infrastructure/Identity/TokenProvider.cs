using System;
using System.Security.Claims;
using System.Text;

using Domain.Entities.Users;

using Infrastructure.Options;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity;

public interface ITokenProvider
{
    string GetToken(User user);
}

public class TokenProvider : ITokenProvider
{
    private readonly JwtOption _jwtOption;

    public TokenProvider(IOptions<JwtOption> jwtOption)
    {
        _jwtOption = jwtOption.Value;
    }

    public string GetToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.SecretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _jwtOption.Issuer,
            Audience = _jwtOption.Audience,
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JsonWebTokenHandler();

        return tokenHandler.CreateToken(tokenDescriptor);
    }
}