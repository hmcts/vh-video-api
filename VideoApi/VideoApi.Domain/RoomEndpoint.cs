using System;

namespace VideoApi.Domain
{
    public class RoomEndpoint : TrackableEntity<long>
    {
        public RoomEndpoint(Guid endpointId)
        {
            EndpointId = endpointId;
        }


        public Guid EndpointId { get; private set; }

    }
}
