using System;
using System.Security.Cryptography;
using System.Text;
using VideoApi.Common.Security.CustomToken;

namespace VideoApi.Common.Security.HashGen
{
    public class HashGenerator
    {
        private readonly ICustomJwtTokenSettings _customJwtTokenSettings;

        public HashGenerator(ICustomJwtTokenSettings customJwtTokenSettings)
        {
            _customJwtTokenSettings = customJwtTokenSettings;
        }

        public string GenerateHash(string participantId)
        {
            var key = Convert.FromBase64String(_customJwtTokenSettings.Secret);
            var stringToHash = $"{DateTime.UtcNow}{DateTime.UtcNow.AddHours(1)}{participantId}";

            var request = Encoding.UTF8.GetBytes(stringToHash);
            using (var hmac = new HMACSHA256(key))
            {
                var computeHash = hmac.ComputeHash(request);
                return Convert.ToBase64String(computeHash);
            }
        }
    }
}