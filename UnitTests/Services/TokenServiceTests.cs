using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Domain.Models;
using Domain.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _tokenService = new TokenService();
        }

        [Fact]
        public void CreateTokenReturnsValidTokenModel()
        {
            // Arrange
            IdentityUser user = new()
            {
                Id = "user1",
                UserName = "johndoe",
                Email = "johndoe@example.com"
            };

            // Act
            TokenModel tokenModel = _tokenService.CreateToken(user);

            // Assert
            Assert.NotNull(tokenModel.Token);
            Assert.True(tokenModel.ExpiresIn > DateTime.UtcNow);
        }

        [Fact]
        public void CreateTokenCreatesValidJwtToken()
        {
            // Arrange
            IdentityUser user = new()
            {
                Id = "user1",
                UserName = "johndoe",
                Email = "johndoe@example.com"
            };

            // Act
            TokenModel tokenModel = _tokenService.CreateToken(user);
            JwtSecurityTokenHandler tokenHandler = new();
            bool isTokenValid = tokenHandler.CanReadToken(tokenModel.Token);

            // Assert
            Assert.True(isTokenValid);
        }

        [Fact]
        public void CreateClaimsReturnsValidClaimsList()
        {
            // Arrange
            IdentityUser user = new()
            {
                Id = "user1",
                UserName = "johndoe",
                Email = "johndoe@example.com"
            };

            // Act
            List<Claim> claims = _tokenService.InvokePrivateMethod<List<Claim>>("CreateClaims", user);

            // Assert
            Assert.NotNull(claims);
            Assert.Equal(5, claims.Count);
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id);
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Jti);
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Iat);
            Assert.Contains(claims, c => c.Type == ClaimTypes.Name && c.Value == user.UserName);
            Assert.Contains(claims, c => c.Type == ClaimTypes.Email && c.Value == user.Email);
        }
        [Fact]
        public void CreateClaimsThrowsExceptionWhenUserIsNull()
        {
            // Arrange
            TokenService tokenService = new();
            IdentityUser user = null;

            // Act and Assert
            Exception exception = Assert.ThrowsAny<Exception>(() =>
                tokenService.InvokePrivateMethod<List<Claim>>("CreateClaims", user));

            Assert.NotNull(exception);
        }



        [Fact]
        public void CreateSigningCredentialsReturnsValidSigningCredentials()
        {
            // Act
            SigningCredentials signingCredentials = _tokenService.InvokePrivateMethod<SigningCredentials>("CreateSigningCredentials");

            // Assert
            Assert.NotNull(signingCredentials);
            Assert.Equal(SecurityAlgorithms.HmacSha256, signingCredentials.Algorithm);
            _ = Assert.IsType<SymmetricSecurityKey>(signingCredentials.Key);
        }
    }
}
