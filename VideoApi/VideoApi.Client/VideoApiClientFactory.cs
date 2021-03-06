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
                ReadResponseAsString = true
            };
            apiClient.JsonSerializerSettings.ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()};
            apiClient.JsonSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            apiClient.JsonSerializerSettings.Converters.Add(new StringEnumConverter());
            return apiClient;
        }
        
        public static VideoApiClient GetClient(string baseUrl, HttpClient httpClient)
        {
            var apiClient = GetClient(httpClient);
            apiClient.BaseUrl = baseUrl;
            return apiClient;
        }
        
        private JsonSerializerSettings ConfigureVhJsonSettings(JsonSerializerSettings jsonSerializerSettings)
        {
            ReadResponseAsString = true;
            jsonSerializerSettings.ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()};
            jsonSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            jsonSerializerSettings.Converters.Add(new StringEnumConverter());
            return jsonSerializerSettings;
        }
    }
}
