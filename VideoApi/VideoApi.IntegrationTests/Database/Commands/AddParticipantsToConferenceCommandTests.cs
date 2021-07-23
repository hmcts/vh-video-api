using System;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using System.Collections.Generic;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Enums;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Extensions;
using Task = System.Threading.Tasks.Task;

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
        public async Task Should_add_participant_to_conference()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var beforeCount = seededConference.GetParticipants().Count;

            var participant = new ParticipantBuilder(true).Build();
            var participants = new List<ParticipantBase>() {participant};
            var command = new AddParticipantsToConferenceCommand(_newConferenceId, participants, new List<LinkedParticipantDto>());

            await _handler.Handle(command);

            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var confParticipants = conference.GetParticipants();
            confParticipants.Any(x => x.Username == participant.Username).Should().BeTrue();
            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().BeGreaterThan(beforeCount);
        }

        [Test]
        public void Should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var participants = new List<ParticipantBase>();

            var command = new AddParticipantsToConferenceCommand(conferenceId, participants, new List<LinkedParticipantDto>());
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }
        
        [Test]
        public async Task Should_add_interpreter_linked_participants()
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceId = seededConference.Id;
            
            var participantA = new ParticipantBuilder(true).Build();
            var participantB = new ParticipantBuilder(true).Build();

            var linkedParticipants = new List<LinkedParticipantDto>()
            {
                new LinkedParticipantDto() { ParticipantRefId = participantA.ParticipantRefId, LinkedRefId = participantB.ParticipantRefId, Type = LinkedParticipantType.Interpreter.MapToDomainEnum()},
                new LinkedParticipantDto() { ParticipantRefId = participantB.ParticipantRefId, LinkedRefId = participantA.ParticipantRefId, Type = LinkedParticipantType.Interpreter.MapToDomainEnum()}
            };

            var participants = new List<ParticipantBase>() {participantA, participantB};

            var command = new AddParticipantsToConferenceCommand(_newConferenceId, participants, linkedParticipants);
            
            await _handler.Handle(command);
            
            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var confParticipants = conference.GetParticipants();
            var linkCount = confParticipants.Sum(x => x.LinkedParticipants.Count);
            linkCount.Should().Be(2);
            
            // verify correct links have been added
            var participantAFromContext = confParticipants.Single(x => x.Id == participantA.Id);
            var participantBFromContext = confParticipants.Single(x => x.Id == participantB.Id);
            participantAFromContext.LinkedParticipants.Should().Contain(x => x.LinkedId == participantBFromContext.Id);
            participantBFromContext.LinkedParticipants.Should().Contain(x => x.LinkedId == participantAFromContext.Id);
        }

        [Test]
        public async Task Should_throw_participant_link_exception_when_id_doesnt_match()
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceId = seededConference.Id;
            
            var participantA = new ParticipantBuilder(true).Build();
            var participantB = new ParticipantBuilder(true).Build();

            var fakeIdA = Guid.NewGuid();
            var fakeIdB = Guid.NewGuid();
            
            var linkedParticipants = new List<LinkedParticipantDto>()
            {
                new LinkedParticipantDto() { ParticipantRefId = fakeIdA, LinkedRefId = participantB.ParticipantRefId, Type = LinkedParticipantType.Interpreter.MapToDomainEnum()},
                new LinkedParticipantDto() { ParticipantRefId = fakeIdB, LinkedRefId = participantA.ParticipantRefId, Type = LinkedParticipantType.Interpreter.MapToDomainEnum()}
            };

            var participants = new List<ParticipantBase>() {participantA, participantB};

            var command = new AddParticipantsToConferenceCommand(_newConferenceId, participants, linkedParticipants);

            var exception = Assert.ThrowsAsync<ParticipantLinkException>(() => _handler.Handle(command));
            exception.LinkRefId.Should().Be(participantB.ParticipantRefId);
            exception.ParticipantRefId.Should().Be(fakeIdA);
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
