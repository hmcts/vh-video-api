using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace VideoApi.Common.Security.Supplier.Base
{
    public abstract class CustomJwtTokenProvider(SupplierConfiguration supplierConfiguration) : ICustomJwtTokenProvider
    {
        public string GenerateApiToken(string claims, int expiresInMinutes)
        {
            byte[] key = Convert.FromBase64String(supplierConfiguration.ApiSecret);
            return BuildToken(claims, expiresInMinutes, key);
        }
        
        public string GenerateSelfTestApiToken(string claims, int expiresInMinutes)
        {
            byte[] key = Convert.FromBase64String(supplierConfiguration.SelfTestApiSecret);
            return BuildToken(claims, expiresInMinutes, key);
        }

        // This is for acceptance tests only... do not use this anywhere else.
        public string GenerateTokenForCallbackEndpoint(string claims, int expiresInMinutes)
        {
            byte[] key = new ASCIIEncoding().GetBytes(supplierConfiguration.CallbackSecret);
            return BuildToken(claims, expiresInMinutes, key);
        }

        protected virtual string BuildToken(string claims, int expiresInMinutes, byte[] key)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, claims)}),
                IssuedAt = DateTime.UtcNow.AddMinutes(-1),
                NotBefore = DateTime.UtcNow.AddMinutes(-1),
                Issuer = supplierConfiguration.Issuer,
                Expires =  DateTime.UtcNow.AddMinutes(expiresInMinutes + 1),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}
