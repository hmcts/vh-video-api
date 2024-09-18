using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace VideoApi.Common.Helpers
{
    public static class ApiRequestHelper
    {
        
        public static T Deserialise<T>(string response)
        {
            return JsonSerializer.Deserialize<T>(response, DefaultSystemTextJsonSerializerSettings());
        }
        
        public static string Serialise(object request)
        {
            return JsonSerializer.Serialize(request, DefaultSystemTextJsonSerializerSettings());
        }
        
        public static string SerialiseRequestToCamelCaseJson(object request)
        {
            return JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            });
        }
        
        public static JsonSerializerOptions DefaultSystemTextJsonSerializerSettings()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            return options;
        }
    }
}
