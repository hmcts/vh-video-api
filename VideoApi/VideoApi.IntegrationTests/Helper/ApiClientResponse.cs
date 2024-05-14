using System.Net.Http;
using System.Threading.Tasks;
using VideoApi.Common.Helpers;

namespace VideoApi.IntegrationTests.Helper
{
    public static class ApiClientResponse
    {
        public static async Task<T> GetResponses<T>(HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return ApiRequestHelper.Deserialise<T>(json);
        }
    }
}
