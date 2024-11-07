using RestSharp;
using Testing.Common.AcCommon;
using Testing.Common.Configuration;
using VideoApi.Services;

namespace VideoApi.AcceptanceTests.Contexts
{
    public class TestContext
    {
        public Config Config { get; set; }
        public RestRequest Request { get; set; }
        public IRestResponse Response { get; set; }
        public Test Test { get; set; }
        public VideoApiTokens Tokens { get; set; }
        public AzureStorageManager AzureStorage { get; set; }

        public RestClient Client()
        {
            var client = new RestClient(Config.Services.VideoApiUrl);
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("Authorization", $"Bearer {Tokens.VideoApiBearerToken}");
            return client;
        }

        public static RestRequest Get(string path)
        {
            return new RestRequest(path, Method.GET);
        }

        public static RestRequest Post(string path, object requestBody)
        {
            var request = new RestRequest(path, Method.POST);            
            request.AddParameter("Application/json", ApiRequestHelper.Serialise(requestBody),
                ParameterType.RequestBody);
            return request;
        }

        public static RestRequest Delete(string path)
        {
            return new RestRequest(path, Method.DELETE);
        }

        public static RestRequest Put(string path, object requestBody)
        {
            var request = new RestRequest(path, Method.PUT);
            request.AddParameter("Application/json", ApiRequestHelper.Serialise(requestBody),
                ParameterType.RequestBody);
            return request;
        }

        public static RestRequest Patch(string path, object requestBody = null)
        {
            var request = new RestRequest(path, Method.PATCH);
            request.AddParameter("Application/json", ApiRequestHelper.Serialise(requestBody),
                ParameterType.RequestBody);
            return request;
        }
    }
}
