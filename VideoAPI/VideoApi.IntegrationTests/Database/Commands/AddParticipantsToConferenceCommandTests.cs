using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using System.Collections.Generic;
using System.Linq;
using Testing.Common.Helper.Builders;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class AddParticipantsToConferenceCommandTests : DatabaseTestsBase
    {
        private AddParticipantsToConferenceCommandHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AddParticipantsToConferenceCommandHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task should_add_participant_to_conference()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var beforeCount = seededConference.GetParticipants().Count;
            
            var participant = new ParticipantBuilder(true).Build();
            var participants = new List<Participant>() {participant};
            var command = new AddParticipantsToConferenceCommand(_newConferenceId, participants);
            
            await _handler.Handle(command);
            
            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var confParticipants = conference.GetParticipants();
            confParticipants.Any(x => x.Username == participant.Username).Should().BeTrue();
            var afterCount = seededConference.GetParticipants().Count;
            afterCount.Should().BeGreaterThan(beforeCount);
        }

        [Test]
        public void should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var participants = new List<Participant>();

            var command = new AddParticipantsToConferenceCommand(conferenceId, participants);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
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