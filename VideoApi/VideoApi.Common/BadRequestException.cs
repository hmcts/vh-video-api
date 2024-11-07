using System;
using System.Runtime.Serialization;

namespace VideoApi.Common
{
    /// <summary>
    /// Exception to throw when input data passed downstream from the api input is in an invalid format
    /// </summary>
    [Serializable]
    public class BadRequestException : Exception
    {
#pragma warning disable SYSLIB0051
        protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051

        public BadRequestException(string message) : base(message) { }
    }
}
