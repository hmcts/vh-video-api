using System;
using System.Linq;
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
    public class GetConferenceByIdQueryTests : DatabaseTestsBase
    {
        private GetConferenceByIdQueryHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetConferenceByIdQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task Should_get_conference_details_by_id()
        {
            var seededConference = await TestDataManager.SeedConference();
            var consultationRoom = await TestDataManager.SeedRoom(new ConsultationRoom(seededConference.Id, VirtualCourtRoomType.Participant, false));
            var consultationRoomParticipant = seededConference.Participants[0];
            await TestDataManager.SeedRoomWithRoomParticipant(consultationRoom.Id, new RoomParticipant(consultationRoomParticipant.Id));

            var participantRoom = await TestDataManager.SeedRoom(new ParticipantRoom(seededConference.Id, "JohSharedRoom1", VirtualCourtRoomType.JudicialShared));
            var participantRoomParticipant = seededConference.Participants.FirstOrDefault(x => x.UserRole == UserRole.JudicialOfficeHolder);
            await TestDataManager.SeedRoomWithRoomParticipant(participantRoom.Id, new RoomParticipant(participantRoomParticipant.Id));

            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var conference = await _handler.Handle(new GetConferenceByIdQuery(_newConferenceId));

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
                participant.GetCurrentRoom().Should().NotBeNullOrEmpty();
                if (consultationRoomParticipant.Id == participant.Id)
                {
                    participant.GetParticipantRoom().Should().BeNull();
                }

                if (participantRoomParticipant.Id == participant.Id)
                {
                    participant.GetParticipantRoom().Should().NotBeNull();
                }
            }

            conference.GetCurrentStatus().Should().BeEquivalentTo(seededConference.GetCurrentStatus());
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
