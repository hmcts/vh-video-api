using System;
using VideoApi.Domain;

namespace VideoApi.Services
{
    public interface IVideoPlatformService
    {
        VirtualCourt BookVirtualCourtroom(Guid conferenceId);
    }
}