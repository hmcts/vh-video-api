using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public class RoomEndpoint : Entity<long>
    {
        public RoomEndpoint(Guid endpointId)
        {
            EndpointId = endpointId;
        }


        public Guid EndpointId { get; private set; }

    }
}
