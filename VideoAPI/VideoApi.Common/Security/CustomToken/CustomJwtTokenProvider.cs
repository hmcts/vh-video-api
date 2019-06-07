using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace VideoApi.Common.Security.CustomToken
{
    public class CustomJwtTokenProvider : ICustomJwtTokenProvider
    {
        private readonly CustomTokenSettings _customTokenSettings;

        public CustomJwtTokenProvider(CustomTokenSettings customTokenSettings)
        {
            _customTokenSettings = customTokenSettings;
        }

        public string GenerateToken(string claims, int expiresInMinutes)
        {
            byte[] key = Convert.FromBase64String(_customTokenSettings.Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, claims) }),
                Audience = _customTokenSettings.Audience,
                Issuer = _customTokenSettings.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

    }
}