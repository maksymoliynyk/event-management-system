using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Options.OptionsSetup;

public class JwtBearerOptionSetup : IConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOption _jwtOption;

    public JwtBearerOptionSetup(JwtOption jwtOption)
    {
        _jwtOption = jwtOption;
    }

    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOption.Issuer,
            ValidAudience = _jwtOption.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOption.SecretKey))
        };
    }
}