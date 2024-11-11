using System;

namespace VideoApi.Common
{
    /// <summary>
    /// Exception to throw when input data passed downstream from the api input is in an invalid format
    /// </summary>
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}
