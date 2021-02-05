using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VideoApi.Services.Helpers
{
    public static class ApiRequestHelper
    {
        public static T Deserialise<T>(string response)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            
            return JsonConvert.DeserializeObject<T>(response, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
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
