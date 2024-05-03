using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace VideoApi.Client
{
    public partial class VideoApiClient
    {
        public static VideoApiClient GetClient(HttpClient httpClient)
        {
            var apiClient = new VideoApiClient(httpClient)
            {
                ReadResponseAsString = true,
                JsonSerializerSettings =
                {
                    ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()},
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                }
            };
            apiClient.JsonSerializerSettings.Converters.Add(new StringEnumConverter());
            return apiClient;
        }
        
        public static VideoApiClient GetClient(string baseUrl, HttpClient httpClient)
        {
            var apiClient = GetClient(httpClient);
            apiClient.BaseUrl = baseUrl;
            return apiClient;
        }
    }
}
