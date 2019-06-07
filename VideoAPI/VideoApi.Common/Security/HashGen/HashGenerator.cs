using System;
using System.Security.Cryptography;
using System.Text;
using VideoApi.Common.Security.CustomToken;

namespace VideoApi.Common.Security.HashGen
{
    public class HashGenerator
    {
        private readonly CustomTokenSettings _customTokenSettings;

        public HashGenerator(CustomTokenSettings customTokenSettings)
        {
            _customTokenSettings = customTokenSettings;
        }

        public string GenerateHash(DateTime expiresOnUtc, string data)
        {
            var key = Convert.FromBase64String(_customTokenSettings.Secret);
            var stringToHash = $"{expiresOnUtc}{data}";

            var request = Encoding.UTF8.GetBytes(stringToHash);
            using (var hmac = new HMACSHA256(key))
            {
                var computedHash = hmac.ComputeHash(request);
                return Convert.ToBase64String(computedHash);
            }
        }
    }
}