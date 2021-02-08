using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace VideoApi.UnitTests.Clients
{
    public sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        public string ReturnContent { get; set; } = "{}";
        
        private HttpResponseMessage Send(HttpRequestMessage request)
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

            if (request.RequestUri.Host.StartsWith($"{HttpStatusCode.NotFound.ToString().ToLower()}.com"))
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Not Found")
                };
            }

            return new HttpResponseMessage {StatusCode = HttpStatusCode.OK, Content = new StringContent(ReturnContent)};
        }
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }
}
