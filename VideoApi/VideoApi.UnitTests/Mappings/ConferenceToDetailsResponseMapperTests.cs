using System.Linq;
using FizzWare.NBuilder;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;
using AudioPlaybackLanguage = VideoApi.Contract.Enums.AudioPlaybackLanguage;
using RoomType = VideoApi.Contract.Enums.RoomType;

namespace VideoApi.UnitTests.Mappings
{
    public class ConferenceToDetailsResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .WithParticipants(3)
                .WithMessages(5)
                .WithTelephoneParticipant("Anonymous")
                .WithInterpreterRoom()
                .Build();
            string conferencePhoneNumber = "+441234567890";
            string conferencePhoneNumberWelsh = "+449876543210";
            string pexipSelfTestNode = "selttest@pexip.node";
            
            var configuration = Builder<VodafoneConfiguration>.CreateNew()
                .With(x => x.ConferencePhoneNumber = conferencePhoneNumber)
                .With(x => x.ConferencePhoneNumberWelsh = conferencePhoneNumberWelsh)
                .With(x => x.PexipSelfTestNode = pexipSelfTestNode).Build();
            
            var response = ConferenceToDetailsResponseMapper.Map(conference, configuration);
            response.Should().BeEquivalentTo(conference, options => options
                .Excluding(x => x.HearingRefId)
                .Excluding(x => x.Participants)
                .Excluding(x => x.ConferenceStatuses)
                .Excluding(x => x.State)
                .Excluding(x => x.InstantMessageHistory)
                .Excluding(x => x.IngestUrl)
                .Excluding(x => x.ActualStartTime)
                .Excluding(x => x.Endpoints)
                .Excluding(x => x.CreatedDateTime)
                .Excluding(x => x.Rooms)
                .Excluding(x => x.UpdatedAt)
                .Excluding(x => x.Supplier)
                .Excluding(x => x.CaseName)
                .Excluding(x => x.CaseNumber)
                .Excluding(x => x.CaseType)
                .Excluding(x => x.HearingVenueName)
                .Excluding(x => x.Supplier)
                .Excluding(x=> x.AudioPlaybackLanguage)
                .Excluding(x => x.IngestUrlFilenamePrefix)
            );
            
            response.TelephoneConferenceId.Should().Be(conference.MeetingRoom.TelephoneConferenceId);
            response.TelephoneConferenceNumbers.Should().Be($"{conferencePhoneNumber},{conferencePhoneNumberWelsh}");
            response.StartedDateTime.Should().HaveValue().And.Be(conference.ActualStartTime);
            response.ClosedDateTime.Should().HaveValue().And.Be(conference.ClosedDateTime);
            response.CurrentStatus.Should().Be((Contract.Enums.ConferenceState)conference.GetCurrentStatus());
            response.Supplier.Should().Be((Contract.Enums.Supplier)conference.Supplier);
            response.AudioPlaybackLanguage.Should().Be((AudioPlaybackLanguage)conference.AudioPlaybackLanguage);
            
            var participants = conference.GetParticipants();
            response.Participants.Should().BeEquivalentTo(participants, options => options
                .Excluding(x => x.ParticipantRefId)
                .Excluding(x => x.ConferenceId)
                .Excluding(x => x.TestCallResultId)
                .Excluding(x => x.TestCallResult)
                .Excluding(x => x.CurrentConsultationRoomId)
                .Excluding(x => x.CurrentConsultationRoom)
                .Excluding(x => x.CurrentRoom)
                .Excluding(x => x.State)
                .Excluding(x => x.LinkedParticipants)
                .Excluding(x => x.RoomParticipants)
                .Excluding(x => x.UpdatedAt)
                .Excluding(x => x.CreatedAt)
                .Excluding(x => x.HearingRole)
                .Excluding(x => x.Endpoint)
                .Excluding(x => x.EndpointId)
            );
            
            var telephoneParticipants = conference.GetTelephoneParticipants();
            response.TelephoneParticipants.Count.Should().Be(telephoneParticipants.Count);
            var telParticipant = response.TelephoneParticipants[0];
            telParticipant.Id.Should().Be(telephoneParticipants[0].Id);
            telParticipant.PhoneNumber.Should().Be(telephoneParticipants[0].TelephoneNumber);
            telParticipant.Room.Should().Be(RoomType.WaitingRoom);
            
            var civilianRoom = response.CivilianRooms[0];
            var room = conference.Rooms.First();
            civilianRoom.Id.Should().Be(room.Id);
            civilianRoom.Label.Should().Be(room.Label);
            civilianRoom.Participants.Select(x => x).Should()
                .BeEquivalentTo(room.RoomParticipants.Select(x => x.ParticipantId));
            
        }
    }
}
