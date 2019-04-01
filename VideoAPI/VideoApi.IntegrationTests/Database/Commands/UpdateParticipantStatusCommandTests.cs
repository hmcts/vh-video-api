using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateParticipantStatusCommandTests : DatabaseTestsBase
    {
        private UpdateParticipantStatusCommandHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateParticipantStatusCommandHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public void should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var participantId = Guid.NewGuid();
            var state = ParticipantState.InConsultation;
            var command = new UpdateParticipantStatusCommand(conferenceId, participantId, state);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task should_throw_participant_not_found_exception_when_participant_does_not_exist()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participantId = Guid.NewGuid();
            var state = ParticipantState.InConsultation;
            var command = new UpdateParticipantStatusCommand(_newConferenceId, participantId, state);
            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task should_update_conference_status()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.GetParticipants().First();
            var state = ParticipantState.InConsultation;

            var beforeCount = participant.GetParticipantStatuses().Count;
            var beforeState = participant.GetCurrentStatus();

            var command = new UpdateParticipantStatusCommand(_newConferenceId, participant.Id, state);
            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedParticipant =
                updatedConference.GetParticipants().Single(x => x.Username == participant.Username);
            var afterCount = updatedParticipant.GetParticipantStatuses().Count;
            var afterState = updatedParticipant.GetCurrentStatus();

            afterCount.Should().BeGreaterThan(beforeCount);
            afterState.Should().NotBe(beforeState);
            afterState.ParticipantState.Should().Be(state);
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