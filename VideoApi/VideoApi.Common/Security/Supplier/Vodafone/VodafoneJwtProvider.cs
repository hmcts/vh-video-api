using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using VideoApi.Common.Security.Supplier.Base;

namespace VideoApi.Common.Security.Supplier.Vodafone;

public interface IVodafoneJwtProvider: ICustomJwtTokenProvider{}

public class VodafoneJwtProvider(VodafoneConfiguration config) : CustomJwtTokenProvider(config), IVodafoneJwtProvider
{
    protected override string BuildToken(string claims, int expiresInMinutes, byte[] key)
    {
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, claims)}),
            IssuedAt = DateTime.UtcNow.AddMinutes(-1),
            NotBefore = DateTime.UtcNow.AddMinutes(-1),
            Issuer = config.Issuer,
            Audience = config.Audience,
            Expires =  DateTime.UtcNow.AddMinutes(expiresInMinutes + 1),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512)
        };

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
        return handler.WriteToken(token);
    }
}
