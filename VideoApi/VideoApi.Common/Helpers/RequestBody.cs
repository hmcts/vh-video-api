using System.Net.Http;
using System.Text;

namespace VideoApi.Common.Helpers
{
    public static class RequestBody
    {
        public static HttpContent Set<T>(T request)
        {
            return new StringContent(ApiRequestHelper.Serialise(request), Encoding.UTF8, "application/json");
        }
    }
}
