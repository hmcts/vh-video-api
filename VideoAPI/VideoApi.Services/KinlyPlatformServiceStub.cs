using System;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Services.Kinly;

namespace VideoApi.Services
{
    public class KinlyPlatformServiceStub : IVideoPlatformService
    {
        public Task<MeetingRoom> BookVirtualCourtroom(Guid conferenceId)
        {
            var adminUri = $"https://ext-node02.hearings.hmcts.net/webapp/#/?conference=ola@hearings.hmcts.net";
            var judgeUri = $"https://ext-node02.hearings.hmcts.net/webapp/#/?conference=ola@hearings.hmcts.net";
            var participantUri = $"https://ext-node02.hearings.hmcts.net/webapp/#/?conference=ola@hearings.hmcts.net";
            var pexipNode = $"join.poc.hearings.hmcts.net";
            return Task.FromResult(new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode));
        }
    }

    public class KinlyPlatformService : IVideoPlatformService
    {
        private readonly IKinlyApiClient _kinlyApiClient;

        public KinlyPlatformService(IKinlyApiClient kinlyApiClient)
        {
            _kinlyApiClient = kinlyApiClient;
        }


        public async Task<MeetingRoom> BookVirtualCourtroom(Guid conferenceId)
        {
            var response = await _kinlyApiClient.CreateHearingAsync(new CreateHearingParams
            {
                Virtual_courtroom_id = conferenceId.ToString()
            });

            var meetingRoom = new MeetingRoom(response.Uris.Admin, response.Uris.Judge, response.Uris.Participant,
                response.Uris.Pexip_node);
            return meetingRoom;
        }
    }
}