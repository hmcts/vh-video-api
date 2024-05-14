using System.Net.Http;
using System.Text;
using VideoApi.Common.Helpers;

namespace VideoApi.IntegrationTests.Helper
{
    public static class RequestBody
    {
        public static HttpContent Set<T>(T request)
        {
            return new StringContent(ApiRequestHelper.Serialise(request), Encoding.UTF8, "application/json");
        }
    }
}
