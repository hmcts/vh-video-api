using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VideoApi.Common.Configuration;
using VideoApi.Domain.Enums;

namespace VideoApi.Common.Security
{
    public class MagicLinksJwtTokenProvider : IMagicLinksJwtTokenProvider
    {
        private readonly MagicLinksConfiguration _magicLinksConfiguration;
        public const int ExpiryTimeInMinutes = 60 * 8;

        public MagicLinksJwtTokenProvider(IOptions<MagicLinksConfiguration> magicLinksConfiguration)
        {
            _magicLinksConfiguration = magicLinksConfiguration.Value;
        }

        public MagicLinksJwtDetails GenerateToken(string name, string userName, UserRole role)
        {
            var key = Convert.FromBase64String(_magicLinksConfiguration.JwtProviderSecret);

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

        private MagicLinksJwtDetails BuildToken(ClaimsIdentity claims, int expiresInMinutes, byte[] key)
        {
            var securityKey = new SymmetricSecurityKey(key);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                NotBefore = DateTime.UtcNow.AddMinutes(-1),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes + 1),
                Issuer = _magicLinksConfiguration.Issuer,
                IssuedAt = DateTime.Now,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(descriptor);

            return new MagicLinksJwtDetails(handler.WriteToken(token), descriptor.Expires.Value);
        }
    }
}
