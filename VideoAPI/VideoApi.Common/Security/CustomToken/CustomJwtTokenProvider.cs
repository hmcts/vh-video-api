using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace VideoApi.Common.Security.CustomToken
{
    public class CustomJwtTokenProvider : ICustomJwtTokenProvider
    {
        private readonly ICustomJwtTokenSettings _customJwtTokenSettings;

        public CustomJwtTokenProvider(ICustomJwtTokenSettings customJwtTokenSettings)
        {
            _customJwtTokenSettings = customJwtTokenSettings;
        }

        public string GenerateToken(string basedOn, int expiresInMinutes)
        {
            byte[] key = Convert.FromBase64String(_customJwtTokenSettings.Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, basedOn) }),
                Audience = _customJwtTokenSettings.Audience,
                Issuer = _customJwtTokenSettings.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

    }
}