using System;
using System.Net;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Kinly;

namespace VideoApi.Services
{
    public class KinlyPlatformService : IVideoPlatformService
    {
        private readonly IKinlyApiClient _kinlyApiClient;

        public KinlyPlatformService(IKinlyApiClient kinlyApiClient)
        {
            _kinlyApiClient = kinlyApiClient;
        }


        public async Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId)
        {
            try
            {
                var response = await _kinlyApiClient.CreateHearingAsync(new CreateHearingParams
                {
                    Virtual_courtroom_id = conferenceId.ToString()
                });

                var meetingRoom = new MeetingRoom(response.Uris.Admin, response.Uris.Judge, response.Uris.Participant,
                    response.Uris.Pexip_node);
                return meetingRoom;
            }
            catch (KinlyApiException e)
            {
                if (e.StatusCode == (int) HttpStatusCode.Conflict)
                {
                    throw new DoubleBookingException(conferenceId, e.Message);
                }

                throw;
            }
        }

        public async Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId)
        {
            try
            {
                var response = await _kinlyApiClient.GetHearingAsync(conferenceId.ToString());
                var meetingRoom = new MeetingRoom(response.Uris.Admin, response.Uris.Judge, response.Uris.Participant,
                    response.Uris.Pexip_node);
                return meetingRoom;
            }
            catch (KinlyApiException e)
            {
                if (e.StatusCode == (int) HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
        }
    }
}