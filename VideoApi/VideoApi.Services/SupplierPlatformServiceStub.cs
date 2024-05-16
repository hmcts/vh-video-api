using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Domain;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Clients;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class SupplierPlatformServiceStub : IVideoPlatformService
    {
        private readonly List<Guid> _bookedGuids = new();
        
        public SupplierConfiguration GetConfig()
        {
            return new KinlyConfiguration();
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

        public Task TransferParticipantAsync(Guid conferenceId, string participantId, string fromRoom, string toRoom)
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

        public Task StartHearingAsync(Guid conferenceId, string triggeredByHostId, IEnumerable<string> participantsToForceTransfer = null,
            Layout layout = Layout.AUTOMATIC, bool muteGuests = false)
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
            const string URL_CONFERENCE = "https://ext-node02.com/webapp/#/?conference=user@hmcts.net";
            var adminUri = URL_CONFERENCE;
            var judgeUri = URL_CONFERENCE;
            var participantUri = URL_CONFERENCE;
            var pexipNode = "join.node.com";
            var telephoneConferenceId = "12345678";
            return new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode, telephoneConferenceId);
        }
    }
}
