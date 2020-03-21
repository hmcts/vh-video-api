using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateParticipantDetailsCommandTests : DatabaseTestsBase
    {
        private UpdateParticipantDetailsCommandHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateParticipantDetailsCommandHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public void Should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var participantId = Guid.NewGuid();
            
            var command = new UpdateParticipantDetailsCommand(conferenceId, participantId, "fullname", "displayname", String.Empty);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_throw_participant_not_found_exception_when_participant_does_not_exist()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participantId = Guid.NewGuid();

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, participantId, "fullname", "displayname", String.Empty);
            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_update_conference_status()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.GetParticipants().First();

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, participant.Id, "fullname", "displayname", String.Empty);
            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedParticipant =
                updatedConference.GetParticipants().Single(x => x.Username == participant.Username);
            updatedParticipant.DisplayName.Should().Be("displayname");
            updatedParticipant.Name.Should().Be("fullname");
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