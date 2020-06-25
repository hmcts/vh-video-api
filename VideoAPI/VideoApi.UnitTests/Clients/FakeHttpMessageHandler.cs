using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace VideoApi.UnitTests.Clients
{
    public sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private static HttpResponseMessage Send(HttpRequestMessage request)
        {
            if (request.RequestUri.Host.StartsWith($"{nameof(Exception).ToLower()}.com"))
            {
                throw new Exception("Exception thrown");
            }

            if (request.RequestUri.Host.StartsWith($"{HttpStatusCode.BadRequest.ToString().ToLower()}.com"))
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest, 
                    Content = new StringContent("Bad request")
                };
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("{}") };
        }
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }
}
