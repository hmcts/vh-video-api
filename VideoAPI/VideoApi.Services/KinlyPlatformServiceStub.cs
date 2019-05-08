using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Services.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    public class KinlyPlatformServiceStub : IVideoPlatformService
    {
        private readonly List<Guid> _bookedGuids;

        public KinlyPlatformServiceStub()
        {
            _bookedGuids = new List<Guid>();
        }
        
        public Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId)
        {
            if(_bookedGuids.Contains(conferenceId))
                throw new DoubleBookingException(conferenceId, "Meeting room already exists");
            
            var meetingRoom = Create();
            _bookedGuids.Add(conferenceId);
            return Task.FromResult(meetingRoom);
        }

        public Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId)
        {
            return Task.FromResult(Create());
        }

        private static MeetingRoom Create()
        {
            var adminUri = $"https://ext-node02.com/webapp/#/?conference=user@email.com";
            var judgeUri = $"https://ext-node02.com/webapp/#/?conference=user@email.com";
            var participantUri = $"https://ext-node02.com/webapp/#/?conference=user@email.com";
            var pexipNode = $"join.node.com";
            return new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode);
        }
    }
}