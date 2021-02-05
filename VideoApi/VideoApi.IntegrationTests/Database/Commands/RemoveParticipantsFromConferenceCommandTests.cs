using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using System.Collections.Generic;
using System.Linq;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class RemoveParticipantsFromConferenceCommandTests : DatabaseTestsBase
    {
        private RemoveParticipantsFromConferenceCommandHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new RemoveParticipantsFromConferenceCommandHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public void Should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var participants = new List<Participant>();

            var command = new RemoveParticipantsFromConferenceCommand(conferenceId, participants);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_remove_participant_from_conference()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var beforeCount = seededConference.GetParticipants().Count;

            var participantToRemove = seededConference.GetParticipants().First();
            var participants = new List<Participant> {participantToRemove};
            var command = new RemoveParticipantsFromConferenceCommand(_newConferenceId, participants);
            await _handler.Handle(command);

            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var confParticipants = conference.GetParticipants();
            confParticipants.Any(x => x.Username == participantToRemove.Username).Should().BeFalse();
            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().BeLessThan(beforeCount);
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
