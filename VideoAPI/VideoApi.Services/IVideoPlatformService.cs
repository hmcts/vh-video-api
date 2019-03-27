using System;
using VideoApi.Domain;

namespace VideoApi.Services
{
    public interface IVideoPlatformService
    {
        MeetingRoom BookVirtualCourtroom(Guid conferenceId);
    }
}