using System;
using System.Net;

namespace VideoApi.Services.Exceptions
{
    public class AudioPlatformException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        
        public AudioPlatformException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
