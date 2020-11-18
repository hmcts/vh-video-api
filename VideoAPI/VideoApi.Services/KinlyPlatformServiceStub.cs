using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Kinly;
using Endpoint = VideoApi.Domain.Endpoint;
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

        public Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId,
            bool audioRecordingRequired,
            string ingestUrl,
            IEnumerable<EndpointDto> endpoints)
        {
            if (_bookedGuids.Contains(conferenceId))
                throw new DoubleBookingException(conferenceId);

            var meetingRoom = Create();
            _bookedGuids.Add(conferenceId);
            return Task.FromResult(meetingRoom);
        }

        public Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId)
        {
            return Task.FromResult(Create());
        }

        public Task<TestCallResult> GetTestCallScoreAsync(Guid participantId,int retryAttempts = 2)
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

        public Task StartEndpointPrivateConsultationAsync(Conference conference, Endpoint endpoint, Participant defenceAdvocate)
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

        public Task UpdateVirtualCourtRoomAsync(Guid conferenceId, bool audioRecordingRequired, IEnumerable<EndpointDto> endpoints)
        {
            return Task.CompletedTask;
        }

        public Task StartHearingAsync(Guid conferenceId, Layout layout = Layout.AUTOMATIC)
        {
            return Task.CompletedTask;
        }

        public Task PauseHearingAsync(Guid conferenceId)
        {
            return Task.CompletedTask;
        }

        public Task EndHearingAsync(Guid conferenceId)
        {
            return Task.CompletedTask;
        }

        public Task SuspendHearingAsync(Guid conferenceId)
        {
            return Task.CompletedTask;
        }

        public Task<HealthCheckResponse> GetPlatformHealthAsync()
        {
            return Task.FromResult(new HealthCheckResponse
            {
                Health_status = PlatformHealth.HEALTHY
            });
        }

        private static MeetingRoom Create()
        {
            var adminUri = "https://ext-node02.com/webapp/#/?conference=user@email.com";
            var judgeUri = "https://ext-node02.com/webapp/#/?conference=user@email.com";
            var participantUri = "https://ext-node02.com/webapp/#/?conference=user@email.com";
            var pexipNode = "join.node.com";
            var pstnPin = "89953313";
            return new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode, pstnPin);
        }
    }
}
