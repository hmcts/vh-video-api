using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VideoApi.Common.Configuration;
using VideoApi.Domain.Enums;

namespace VideoApi.Common.Security
{
    public class QuickLinksJwtTokenProvider : IQuickLinksJwtTokenProvider
    {
        private readonly QuickLinksConfiguration _quickLinksConfiguration;
        public const int ExpiryTimeInMinutes = 60 * 8;

        public QuickLinksJwtTokenProvider(IOptions<QuickLinksConfiguration> quickLinksConfiguration)
        {
            _quickLinksConfiguration = quickLinksConfiguration.Value;
        }

        public QuickLinksJwtDetails GenerateToken(string name, string userName, UserRole role)
        {
            var key = Convert.FromBase64String(_quickLinksConfiguration.RsaPrivateKey);

            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.GivenName, name),
                new Claim(ClaimTypes.Surname, name),
                new Claim("preferred_username", userName),
                new Claim("name", name), // Display Name
                new Claim(ClaimTypes.Role, role.ToString())
            });
            
            return BuildToken(claims, ExpiryTimeInMinutes, key);
        }

        private QuickLinksJwtDetails BuildToken(ClaimsIdentity claims, int expiresInMinutes, byte[] key)
        {
            using var rsa = RSA.Create(2048);
            rsa.ImportRSAPrivateKey(key, out int _);
            
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                NotBefore = DateTime.UtcNow.AddMinutes(-1),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes + 1),
                Issuer = _quickLinksConfiguration.Issuer,
                IssuedAt = DateTime.Now,
                Audience = _quickLinksConfiguration.ValidAudience,
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha512)
                {
                    CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
                }
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(descriptor);

            return new QuickLinksJwtDetails(handler.WriteToken(token), descriptor.Expires.Value);
        }
    }
}
