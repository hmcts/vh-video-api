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
        public static ConferenceDetailsResponse Map(Conference conference, SupplierConfiguration configuration)
        {
            var allInterpreterRooms = conference.Rooms.OfType<ParticipantRoom>().ToList();
            var interpreterRooms = allInterpreterRooms.Select(RoomToCivilianRoomResponseMapper.MapToResponse).ToList();
            var phoneNumbers = $"{configuration.ConferencePhoneNumber},{configuration.ConferencePhoneNumberWelsh}";
            var pexipSelfTestNode = configuration.PexipSelfTestNode;
            
            var response = new ConferenceDetailsResponse();
            response.Id = conference.Id;
            response.HearingId = conference.HearingRefId;
            response.ScheduledDateTime = conference.ScheduledDateTime;
            response.StartedDateTime = conference.ActualStartTime;
            response.ClosedDateTime = conference.ClosedDateTime;
            response.ScheduledDuration = conference.ScheduledDuration;
            response.CurrentStatus = conference.GetCurrentStatus().MapToContractEnum();
            response.Participants = MapParticipants(conference.Participants, allInterpreterRooms);
            response.TelephoneParticipants = conference.GetTelephoneParticipants().Select(TelephoneParticipantMapper.Map).ToList();
            response.MeetingRoom = MeetingRoomToResponseMapper.MapVirtualCourtToResponse(conference.GetMeetingRoom());
            response.Endpoints = conference.GetEndpoints().Select(EndpointToResponseMapper.MapEndpointResponse).ToList();
            response.AudioRecordingRequired = conference.AudioRecordingRequired;
            response.CivilianRooms = interpreterRooms;
            response.IngestUrl = conference.IngestUrl;
            response.IsWaitingRoomOpen = conference.IsConferenceAccessible();
            response.TelephoneConferenceId = conference.MeetingRoom.TelephoneConferenceId;
            response.TelephoneConferenceNumbers = phoneNumbers;
            response.CaseName = conference.CaseName;
            response.Supplier = (Contract.Enums.Supplier)conference.Supplier;
            response.ConferenceRoomType = (Contract.Enums.ConferenceRoomType)conference.ConferenceRoomType;
            response.AudioPlaybackLanguage = (Contract.Enums.AudioPlaybackLanguage)conference.AudioPlaybackLanguage;
            
            if (response.MeetingRoom != null)
                response.MeetingRoom.PexipSelfTestNode = pexipSelfTestNode;
            
            return response;
        }
        
        private static List<ParticipantResponse> MapParticipants(IList<ParticipantBase> participants, List<ParticipantRoom> interpreterRooms)
        {
            return participants
                .OrderBy(p=> p.Name)
                .Select(x =>
                {
                    var interpreterRoom =
                        interpreterRooms.SingleOrDefault(r => r.DoesParticipantExist(new RoomParticipant(x.Id)));
                    return ParticipantResponseMapper.Map(x, interpreterRoom);
                }).ToList();
        }
    }
}
