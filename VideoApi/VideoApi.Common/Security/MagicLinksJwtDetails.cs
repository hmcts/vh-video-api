using System;

namespace VideoApi.Common.Security
{
    public class MagicLinksJwtDetails
    {
        public string Token { get; }
        public DateTime Expiry { get; }

        public MagicLinksJwtDetails(string token, DateTime expiry)
        {
            Token = token;
            Expiry = expiry;
        }
    }
}
