using System;
using System.Runtime.Serialization;

namespace VideoApi.DAL.Exceptions
{
    [Serializable]
    public class EndpointNotFoundException : Exception
    {
        public EndpointNotFoundException(Guid endpointId) : base($"Endpoint {endpointId} does not exist")
        {
        }

        public EndpointNotFoundException(string sipAddress) : base($"Endpoint {sipAddress} does not exist")
        {
        }
        
        protected EndpointNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
