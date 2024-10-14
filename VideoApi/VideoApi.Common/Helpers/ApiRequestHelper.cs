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
        
        /// <summary>
        /// The function SerialiseRequestToCamelCaseJson serializes an object to JSON format using camel
        /// case naming conventions and additional options.
        /// </summary>
        /// <param name="request">The `SerialiseRequestToCamelCaseJson` method serializes an object to a
        /// JSON string using camel case naming convention and other specified options. The `request`
        /// parameter is the object that you want to serialize to JSON.</param>
        /// <returns>
        /// A JSON string representation of the provided object `request`, serialized in camel case
        /// format with indentation, ignoring null values, and converting enums to strings in camel
        /// case.
        /// </returns>
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
        
        /// <summary>
        /// The function returns default settings for the System.Text.Json serializer in C# with
        /// specific configurations.
        /// </summary>
        /// <returns>
        /// The method `DefaultSystemTextJsonSerializerSettings` returns a `JsonSerializerOptions`
        /// object with specific settings configured for the System.Text.Json serializer.
        /// </returns>
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
