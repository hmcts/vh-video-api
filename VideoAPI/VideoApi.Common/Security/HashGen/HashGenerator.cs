using System;
using System.Security.Cryptography;
using System.Text;
using VideoApi.Common.Security.CustomToken;

namespace VideoApi.Common.Security.HashGen
{
    public class HashGenerator
    {
        private readonly CustomJwtTokenSettings _customJwtTokenSettings;

        public HashGenerator(CustomJwtTokenSettings customJwtTokenSettings)
        {
            _customJwtTokenSettings = customJwtTokenSettings;
        }

        public string GenerateHash(DateTime expiresOnUtc, string data)
        {
            var key = Convert.FromBase64String(_customJwtTokenSettings.Secret);
            var stringToHash = $"{expiresOnUtc}{data}";

            var request = Encoding.UTF8.GetBytes(stringToHash);
            using (var hmac = new HMACSHA256(key))
            {
                var computeHash = hmac.ComputeHash(request);
                return Convert.ToBase64String(computeHash);
            }
        }
    }
}