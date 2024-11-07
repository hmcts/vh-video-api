using System;

namespace VideoApi.DAL.Exceptions
{
    public class EndpointNotFoundException : EntityNotFoundException
    {
        public EndpointNotFoundException(Guid endpointId) : base($"Endpoint {endpointId} does not exist")
        {
        }

        public EndpointNotFoundException(string sipAddress) : base($"Endpoint {sipAddress} does not exist")
        {
        }
    }
}
