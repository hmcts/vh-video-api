using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using VideoApi.Services.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class KinlyPlatformServiceStub : IVideoPlatformService
    {
        private readonly List<Guid> _bookedGuids;

        public KinlyPlatformServiceStub()
        {
            _bookedGuids = new List<Guid>();
        }

        public Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId, bool audioRecordingRequired, string ingestUrl)
        {
            if (_bookedGuids.Contains(conferenceId))
                throw new DoubleBookingException(conferenceId, "Meeting room already exists");

            var meetingRoom = Create();
            _bookedGuids.Add(conferenceId);
            return Task.FromResult(meetingRoom);
        }

        public Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId)
        {
            return Task.FromResult(Create());
        }

        public Task<TestCallResult> GetTestCallScoreAsync(Guid participantId)
        {
            return Task.FromResult<TestCallResult>(null);
        }

        public Task TransferParticipantAsync(Guid conferenceId, Guid participantId, RoomType fromRoom, RoomType toRoom)
        {
            return Task.CompletedTask;
        }

        public Task StartPrivateConsultationAsync(Conference conference, Participant requestedBy, Participant requestedFor)
        {
            return Task.CompletedTask;
        }

        public Task StopPrivateConsultationAsync(Conference conference, RoomType consultationRoom)
        {
            return Task.CompletedTask;
        }

        public Task DeleteVirtualCourtRoomAsync(Guid conferenceId)
        {
            return Task.CompletedTask;
        }

        public Task UpdateVirtualCourtRoomAsync(Guid conferenceId, bool audioRecordingRequired)
        {
            return Task.CompletedTask;
        }

        public Task StartHearingAsync(Guid conferenceId)
        {
            return Task.CompletedTask;
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
