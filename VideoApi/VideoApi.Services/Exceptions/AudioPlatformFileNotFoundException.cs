using System;
using System.Net;

namespace VideoApi.Services.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly

    public class AudioPlatformFileNotFoundException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public AudioPlatformFileNotFoundException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

    }
}
