using System;
using VideoApi.Domain;

namespace VideoApi.Services
{
    public class KinlyPlatformServiceStub : IVideoPlatformService
    {
        public MeetingRoom BookVirtualCourtroom(Guid conferenceId)
        {
            var adminUri = $"https://ext-node02.hearings.hmcts.net/webapp/#/?conference=ola@hearings.hmcts.net";
            var judgeUri = $"https://ext-node02.hearings.hmcts.net/webapp/#/?conference=ola@hearings.hmcts.net";
            var participantUri = $"https://ext-node02.hearings.hmcts.net/webapp/#/?conference=ola@hearings.hmcts.net";
            var pexipNode = $"join.poc.hearings.hmcts.net";
            return new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode);
        }
    }
}