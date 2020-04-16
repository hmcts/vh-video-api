using System;
using System.Net;

namespace VideoApi.Services.Exceptions
{
    public class StreamingEngineException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        
        public StreamingEngineException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
