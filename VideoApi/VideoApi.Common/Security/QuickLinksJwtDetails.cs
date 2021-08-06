using System;

namespace VideoApi.Common.Security
{
    public class QuickLinksJwtDetails
    {
        public string Token { get; }
        public DateTime Expiry { get; }

        public QuickLinksJwtDetails(string token, DateTime expiry)
        {
            Token = token;
            Expiry = expiry;
        }
    }
}
