using System;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using VideoApi.Common.Configuration;

namespace VideoApi.Common.Security
{
    public interface ITokenProvider
    {
        string GetClientAccessToken(string clientId, string clientSecret, string clientResource);
        AuthenticationResult GetAuthorisationResult(string clientId, string clientSecret, string clientResource);
    }

    public class AzureTokenProvider : ITokenProvider
    {
        private readonly AzureAdConfiguration _securitySettings;

        public AzureTokenProvider(IOptions<AzureAdConfiguration> environmentConfiguration)
        {
            _securitySettings = environmentConfiguration.Value;
        }

        public string GetClientAccessToken(string clientId, string clientSecret, string clientResource)
        {
            var result = GetAuthorisationResult(clientId, clientSecret, clientResource);
            return result.AccessToken;
        }

        public AuthenticationResult GetAuthorisationResult(string clientId, string clientSecret, string clientResource)
        {
            AuthenticationResult result;
            var credential = new ClientCredential(clientId, clientSecret);
            var authContext = new AuthenticationContext($"{_securitySettings.Authority}{_securitySettings.TenantId}");

            try
            {
                result = authContext.AcquireTokenAsync(clientResource, credential).Result;
            }
            catch (AdalException)
            {
                throw new UnauthorizedAccessException();
            }

            return result;
        }
    }
}
