using System.Text.Json;
using System.Text.Json.Serialization;

namespace VideoApi.Common.Helpers
{
    public static class ApiRequestHelper
    {
        private static readonly JsonSerializerOptions DefaultSettings = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        private static JsonSerializerOptions DefaultSystemTextJsonSerializerSettings() => DefaultSettings;
        private static JsonSerializerOptions DefaultSystemTextJsonSerializerSettingsForSupplier() => new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper)
            }
        };

        public static T Deserialise<T>(string response)
        {
            return JsonSerializer.Deserialize<T>(response, DefaultSystemTextJsonSerializerSettings());
        }
        
        public static string Serialise(object request)
        {
            return JsonSerializer.Serialize(request, DefaultSystemTextJsonSerializerSettings());
        }
        
        public static string SerialiseForSupplier(object request)
        {
            return JsonSerializer.Serialize(request, DefaultSystemTextJsonSerializerSettingsForSupplier());
        }
        
        public static T DeserialiseForSupplier<T>(string response)
        {
            return JsonSerializer.Deserialize<T>(response, DefaultSystemTextJsonSerializerSettingsForSupplier());
        }
    }
}
