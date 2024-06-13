using FizzWare.NBuilder;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class ConferenceToSummaryMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .WithParticipant(UserRole.Judge, "Judge")
                .WithParticipants(3)
                .Build();

            const string conferencePhoneNumber = "+441234567890";
            const string conferencePhoneNumberWelsh = "+449876543210";
            var configuration = Builder<KinlyConfiguration>.CreateNew()
                .With(x => x.ConferencePhoneNumber = conferencePhoneNumber)
                .With(x => x.ConferencePhoneNumberWelsh = conferencePhoneNumberWelsh).Build();

            var response = ConferenceForAdminResponseMapper.MapConferenceToAdminResponse(conference, configuration);
            response.Should().BeEquivalentTo(conference, options => options
                .Excluding(x => x.HearingRefId)
                .Excluding(x => x.Participants)
                .Excluding(x => x.ConferenceStatuses)
                .Excluding(x => x.State)
                .Excluding(x => x.InstantMessageHistory)
                .Excluding(x => x.IngestUrl)
                .Excluding(x => x.AudioRecordingRequired)
                .Excluding(x => x.Id)
                .Excluding(x => x.ActualStartTime)
                .Excluding(x => x.MeetingRoom)
                .Excluding(x => x.Endpoints)
                .Excluding(x => x.Rooms)
                .Excluding(x => x.UpdatedAt)
            );
            
            response.StartedDateTime.Should().Be(conference.ActualStartTime);
            response.Status.Should().Be((Contract.Enums.ConferenceState)conference.GetCurrentStatus());
            response.ClosedDateTime.Should().HaveValue().And.Be(conference.ClosedDateTime);
            response.TelephoneConferenceId.Should().Be(conference.MeetingRoom.TelephoneConferenceId);
            response.TelephoneConferenceNumbers.Should().Be($"{conferencePhoneNumber},{conferencePhoneNumberWelsh}");
            response.CreatedDateTime.Should().Be(conference.CreatedDateTime);
        }
    }
}
