using System;
using System.Security.Cryptography;
using System.Text;
using VideoApi.Common.Security.CustomToken;

namespace VideoApi.Common.Security.HashGen
{
    public class MessageEncryptor
    {
        private readonly CustomJwtTokenSettings _customJwtTokenSettings;

        public MessageEncryptor(CustomJwtTokenSettings customJwtTokenSettings)
        {
            _customJwtTokenSettings = customJwtTokenSettings;
        }

        public string HashRequestTarget(string requestTarget)
        {
            var key = Convert.FromBase64String(_customJwtTokenSettings.Secret);
            var requestUri = System.Web.HttpUtility.UrlEncode(requestTarget);

            var request = Encoding.UTF8.GetBytes(requestUri);
            using (var hmac = new HMACSHA256(key))
            {
                var computeHash = hmac.ComputeHash(request);
                return Convert.ToBase64String(computeHash);
            }
        }

    }
}