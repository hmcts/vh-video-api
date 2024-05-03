using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetConferenceByIdForEventsQueryTests : DatabaseTestsBase
    {
        private GetConferenceByIdForEventQueryHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetConferenceByIdForEventQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task Should_get_conference_details_by_id()
        {
            var seededConference = await TestDataManager.SeedConference();
            var room = await TestDataManager.SeedRoom(new ConsultationRoom(seededConference.Id, VirtualCourtRoomType.Participant, false));
            await TestDataManager.SeedRoomWithRoomParticipant(room.Id, new RoomParticipant(seededConference.Participants[0].Id));

            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var conference = await _handler.Handle(new GetConferenceByIdForEventQuery(_newConferenceId));

            conference.Should().NotBeNull();

            conference.CaseType.Should().Be(seededConference.CaseType);
            conference.CaseNumber.Should().Be(seededConference.CaseNumber);
            conference.CaseName.Should().Be(seededConference.CaseName);
            conference.ScheduledDuration.Should().Be(seededConference.ScheduledDuration);
            conference.ScheduledDateTime.Should().Be(seededConference.ScheduledDateTime);
            conference.HearingRefId.Should().Be(seededConference.HearingRefId);
            conference.AudioRecordingRequired.Should().Be(seededConference.AudioRecordingRequired);
            conference.IngestUrl.Should().Be(seededConference.IngestUrl);

            var participants = conference.GetParticipants();
            participants.Should().NotBeNullOrEmpty();
            foreach (var participant in participants)
            {
                participant.Name.Should().NotBeNullOrEmpty();
                participant.Username.Should().NotBeNullOrEmpty();
                participant.DisplayName.Should().NotBeNullOrEmpty();
                participant.ParticipantRefId.Should().NotBeEmpty();
                participant.UserRole.Should().NotBe(UserRole.None);

                if (participant is Participant participantCasted)
                {
                    participantCasted.CaseTypeGroup.Should().NotBeNullOrEmpty();
                    if (participant.UserRole == UserRole.Representative)
                    {
                        participantCasted.Representee.Should().NotBeNullOrEmpty();
                    }
                }
            }

            conference.GetCurrentStatus().Should().Be(seededConference.GetCurrentStatus());
            conference.Rooms.Count.Should().Be(1);
            conference.GetEndpoints().Count.Should().Be(0);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
            }
        }
    }
}
