using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Domain.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

#pragma warning disable CA1822

namespace Domain.Services
{
    public class TokenService
    {
        private const int ExpirationMinutes = 30;
        public virtual TokenModel CreateToken(IdentityUser user)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            JwtSecurityToken token = CreateJwtToken(
                CreateClaims(user),
                CreateSigningCredentials(),
                expiration
            );
            JwtSecurityTokenHandler tokenHandler = new();

            return new TokenModel
            {
                Token = tokenHandler.WriteToken(token),
                ExpiresIn = expiration
            };
        }

        private static JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
            DateTime expiration)
        {
            return new(
                "apiWithAuthBackend",
                "apiWithAuthBackend",
                claims,
                expires: expiration,
                signingCredentials: credentials
            );
        }

        private static List<Claim> CreateClaims(IdentityUser user)
        {
            try
            {
                List<Claim> claims = new()
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };
                return claims;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private static SigningCredentials CreateSigningCredentials()
        {
            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("this is my custom Secret key for authentication")
                ),
                SecurityAlgorithms.HmacSha256
            );
        }
    }
}