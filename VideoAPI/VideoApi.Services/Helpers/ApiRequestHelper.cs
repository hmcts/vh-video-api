using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VideoApi.Services.Helpers
{
    public static class ApiRequestHelper
    {
        public static T DeserialiseSnakeCaseJsonToResponse<T>(string response)
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
    }
}
