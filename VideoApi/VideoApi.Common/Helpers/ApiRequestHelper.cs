using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace VideoApi.Common.Helpers
{
    public static class ApiRequestHelper
    {
        private static JsonSerializerSettings DefaultNewtonsoftSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()},
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Formatting = Formatting.Indented
            };
            
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            
            return settings;
        }
        
        public static T Deserialise<T>(string response)
        {
            return JsonConvert.DeserializeObject<T>(response, DefaultNewtonsoftSerializerSettings());
        }
        
        public static string Serialise(object request)
        {
            return JsonConvert.SerializeObject(request, DefaultNewtonsoftSerializerSettings());
        }
        
        public static string SerialiseRequestToCamelCaseJson(object request)
        {
            return JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });
        }
    }
}
