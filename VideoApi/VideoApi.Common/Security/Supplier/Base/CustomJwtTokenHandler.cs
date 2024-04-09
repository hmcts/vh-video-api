using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace VideoApi.Common.Security.Supplier.Base
{
    public abstract class CustomJwtTokenHandler : ICustomJwtTokenHandler
    {
        private readonly SupplierConfiguration _supplierConfiguration;

        protected CustomJwtTokenHandler(SupplierConfiguration supplierConfiguration)
        {
            _supplierConfiguration = supplierConfiguration;
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token))
            {
                return null;
            }

            var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            if (jwtToken == null)
            {
                return null;
            }

            byte[] key = Convert.FromBase64String(_supplierConfiguration.ApiSecret);
            var parameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidIssuers = new[] {_supplierConfiguration.Issuer},
                ValidateIssuer = true,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key)

            };

            ClaimsPrincipal principal;
            try
            {
                principal = tokenHandler.ValidateToken(token, parameters, out var securityToken);
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return null;
            }

            return principal;
        }

        public bool IsValidToken(string token)
        {
            var principal = GetPrincipal(token);

            if (principal?.Identity != null && principal.Identity is ClaimsIdentity claimsIdentity)
            {
                var identity = claimsIdentity;

                return identity.FindFirst(ClaimTypes.Name) != null;
            }

            return false;
        }
    }
}
