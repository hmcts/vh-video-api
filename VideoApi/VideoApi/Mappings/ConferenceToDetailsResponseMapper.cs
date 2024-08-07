using System.Collections.Generic;
using System.Linq;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;
namespace VideoApi.Mappings
{
    public static class ConferenceToDetailsResponseMapper
    {
        public static ConferenceDetailsResponse MapConferenceToResponse(Conference conference, SupplierConfiguration configuration)
        {
            var allInterpreterRooms = conference.Rooms.OfType<ParticipantRoom>().ToList();
            var interpreterRooms = allInterpreterRooms.Select(RoomToCivilianRoomResponseMapper.MapToResponse).ToList();
            var phoneNumbers = $"{configuration.ConferencePhoneNumber},{configuration.ConferencePhoneNumberWelsh}";
            var pexipSelfTestNode = configuration.PexipSelfTestNode;

            var response = new ConferenceDetailsResponse
            {
                Id = conference.Id,
                HearingId = conference.HearingRefId,
                ScheduledDateTime = conference.ScheduledDateTime,
                StartedDateTime = conference.ActualStartTime,
                ClosedDateTime = conference.ClosedDateTime,
                ScheduledDuration = conference.ScheduledDuration,
                CurrentStatus = conference.GetCurrentStatus().MapToContractEnum(),
                Participants = MapParticipants(conference.Participants, allInterpreterRooms),
                MeetingRoom = MeetingRoomToResponseMapper.MapVirtualCourtToResponse(conference.GetMeetingRoom()),
                Endpoints = conference.GetEndpoints().Select(EndpointToResponseMapper.MapEndpointResponse).ToList(),
                AudioRecordingRequired = conference.AudioRecordingRequired,
                CivilianRooms = interpreterRooms,
                IngestUrl = conference.IngestUrl,
                IsWaitingRoomOpen = conference.IsConferenceAccessible(),
                TelephoneConferenceId = conference.MeetingRoom.TelephoneConferenceId,
                TelephoneConferenceNumbers = phoneNumbers,
                CaseName = conference.CaseName,
            };

            if (response.MeetingRoom != null)
            {
                response.MeetingRoom.PexipSelfTestNode = pexipSelfTestNode;
            }
            
            return response;
        }

        private static List<ParticipantResponse> MapParticipants(IList<ParticipantBase> participants, List<ParticipantRoom> interpreterRooms)
        {
            return participants.Select(x =>
            {
                var interpreterRoom =
                    interpreterRooms.SingleOrDefault(r => r.DoesParticipantExist(new RoomParticipant(x.Id)));
                return ParticipantResponseMapper.Map(x, interpreterRoom);
            }).ToList();
        }
    }
}
