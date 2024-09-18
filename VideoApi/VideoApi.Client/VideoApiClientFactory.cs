using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VideoApi.Client
{
    [ExcludeFromCodeCoverage]
    public partial class VideoApiClient
    {
        public static VideoApiClient GetClient(HttpClient httpClient)
        {
            var apiClient = new VideoApiClient(httpClient)
            {
                ReadResponseAsString = true,
                JsonSerializerSettings =
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                }
            };
            return apiClient;
        }
        
        static partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
        {
            settings.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            settings.WriteIndented = true;
            settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            settings.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }
        
        public static VideoApiClient GetClient(string baseUrl, HttpClient httpClient)
        {
            var apiClient = GetClient(httpClient);
            apiClient.BaseUrl = baseUrl;
            return apiClient;
        }
    }
}
