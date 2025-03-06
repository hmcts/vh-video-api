using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

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

            var command = new UpdateParticipantDetailsCommand(conferenceId, participantId,
                "displayname", "failed@hmcts.net", new List<LinkedParticipantDto>(),
                UserRole.Individual, "Individual");
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_throw_participant_not_found_exception_when_participant_does_not_exist()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participantId = Guid.NewGuid();

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, participantId,
                "displayname", "failed@hmcts.net", new List<LinkedParticipantDto>(),
                UserRole.Individual, "Individual");
            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_update_participant_details()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.GetParticipants().First();

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, participant.Id,
                "displayname", "new@hmcts.net",
                new List<LinkedParticipantDto>(),
                UserRole.Individual, "Litigant in person");
            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedParticipant =
                updatedConference.GetParticipants().Single(x => x.Username == participant.Username);
            updatedParticipant.DisplayName.Should().Be("displayname");

            if (updatedParticipant is Participant updatedParticipantCasted)
            {
                updatedParticipantCasted.ContactEmail.Should().Be("new@hmcts.net");
                updatedParticipantCasted.UserRole.Should().Be(UserRole.Individual);
                updatedParticipantCasted.HearingRole.Should().Be("Litigant in person");
            }

            updatedParticipant.UpdatedAt.Should().BeAfter(updatedParticipant.CreatedAt.Value);
        }

        [Test]
        public async Task Should_update_participant_details_and_linked_participants()
        {
            var conference = new ConferenceBuilder(true, null, DateTime.UtcNow.AddMinutes(5))
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();

            var participantA = new ParticipantBuilder(true).Build();
            var participantB = new ParticipantBuilder(true).Build();
            var participantC = new ParticipantBuilder(true).Build();

            participantA.LinkedParticipants.Add(new LinkedParticipant(participantA.Id, participantB.Id, LinkedParticipantType.Interpreter));
            participantB.LinkedParticipants.Add(new LinkedParticipant(participantB.Id, participantA.Id, LinkedParticipantType.Interpreter));

            conference.AddParticipant(participantA);
            conference.AddParticipant(participantB);
            conference.Participants.Add(participantC);

            var seededConference = await TestDataManager.SeedConference(conference);

            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var participant = seededConference.GetParticipants().First();

            var newLinkedParticipants = new List<LinkedParticipantDto>()
            {
                new LinkedParticipantDto()
                {
                    ParticipantRefId = participantC.ParticipantRefId,
                    LinkedRefId = participantA.ParticipantRefId,
                    Type = LinkedParticipantType.Interpreter
                },
                new LinkedParticipantDto()
                {
                    ParticipantRefId = participantA.ParticipantRefId,
                    LinkedRefId = participantC.ParticipantRefId,
                    Type = LinkedParticipantType.Interpreter
                }
            };

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, participant.Id,
                "displayname", "new@hmcts.net", newLinkedParticipants,
                UserRole.Individual, "Litigant in person");

            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedParticipantA =
                updatedConference.GetParticipants().Single(x => x.Username == participantA.Username);

            updatedParticipantA.LinkedParticipants.Should().NotContain(x => x.LinkedId == participantB.Id);
            updatedParticipantA.LinkedParticipants.Should().Contain(x => x.LinkedId == participantC.Id);

            var updatedParticipantB =
                updatedConference.GetParticipants().Single(x => x.Username == participantB.Username);

            updatedParticipantB.LinkedParticipants.Should().BeEmpty();

            var updatedParticipantC =
                updatedConference.GetParticipants().Single(x => x.Username == participantC.Username);

            updatedParticipantC.LinkedParticipants.Should().NotContain(x => x.LinkedId == participantB.Id);
            updatedParticipantC.LinkedParticipants.Should().Contain(x => x.LinkedId == participantA.Id);
        }

        [Test]
        public async Task
            Should_update_participant_details_and_linked_participants_When_linked_participant_only_on_one_participant()
        {
            var conference = new ConferenceBuilder(true, null, DateTime.UtcNow.AddMinutes(5))
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();

            var participantA = new ParticipantBuilder(true).Build();
            var participantB = new ParticipantBuilder(true).Build();
            var participantC = new ParticipantBuilder(true).Build();

            participantA.LinkedParticipants.Add(new LinkedParticipant(participantA.Id, participantB.Id,
                LinkedParticipantType.Interpreter));

            conference.AddParticipant(participantA);
            conference.AddParticipant(participantB);
            conference.Participants.Add(participantC);

            var seededConference = await TestDataManager.SeedConference(conference);

            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var participant = seededConference.GetParticipants().First();

            var newLinkedParticipants = new List<LinkedParticipantDto>()
            {
                new LinkedParticipantDto()
                {
                    ParticipantRefId = participantA.ParticipantRefId,
                    LinkedRefId = participantC.ParticipantRefId,
                    Type = LinkedParticipantType.Interpreter
                }
            };

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, participant.Id,
                "displayname", "new@hmcts.net", newLinkedParticipants,
                UserRole.Individual, "Litigant in person");

            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedParticipantA =
                updatedConference.GetParticipants().Single(x => x.Username == participantA.Username);

            updatedParticipantA.LinkedParticipants.Should().NotContain(x => x.LinkedId == participantB.Id);
            updatedParticipantA.LinkedParticipants.Should().Contain(x => x.LinkedId == participantC.Id);

            var updatedParticipantB =
                updatedConference.GetParticipants().Single(x => x.Username == participantB.Username);

            updatedParticipantB.LinkedParticipants.Should().BeEmpty();

            var updatedParticipantC =
                updatedConference.GetParticipants().Single(x => x.Username == participantC.Username);

            updatedParticipantC.LinkedParticipants.Should().NotContain(x => x.LinkedId == participantB.Id);
            updatedParticipantC.LinkedParticipants.Should().Contain(x => x.LinkedId == participantA.Id);
        }

        [Test]
        public async Task Should_throw_participant_link_exception_when_id_doesnt_match()
        {
            var conference = new ConferenceBuilder(true, null, DateTime.UtcNow.AddMinutes(5))
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();

            var participantA = new ParticipantBuilder(true).Build();
            var participantB = new ParticipantBuilder(true).Build();
            var participantC = new ParticipantBuilder(true).Build();

            participantA.LinkedParticipants.Add(new LinkedParticipant(participantA.Id, participantB.Id,
                LinkedParticipantType.Interpreter));
            participantB.LinkedParticipants.Add(new LinkedParticipant(participantB.Id, participantA.Id,
                LinkedParticipantType.Interpreter));

            conference.AddParticipant(participantA);
            conference.AddParticipant(participantB);
            conference.Participants.Add(participantC);

            var seededConference = await TestDataManager.SeedConference(conference);

            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var participant = seededConference.GetParticipants().First();

            var fakeIdA = Guid.NewGuid();
            var fakeIdC = Guid.NewGuid();

            var newLinkedParticipants = new List<LinkedParticipantDto>()
            {
                new LinkedParticipantDto()
                {
                    ParticipantRefId = fakeIdC,
                    LinkedRefId = participantA.ParticipantRefId,
                    Type = LinkedParticipantType.Interpreter
                },
                new LinkedParticipantDto()
                {
                    ParticipantRefId = fakeIdA,
                    LinkedRefId = participantC.ParticipantRefId,
                    Type = LinkedParticipantType.Interpreter
                }
            };

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, participant.Id,
                "displayname", "new@hmcts.net", newLinkedParticipants,
                UserRole.Individual, "Litigant in person");

            var exception = Assert.ThrowsAsync<ParticipantLinkException>(() => _handler.Handle(command));
            exception.LinkRefId.Should().Be(participantA.ParticipantRefId);
            exception.ParticipantRefId.Should().Be(fakeIdC);
        }

        [Test]
        public async Task Should_update_participant_username_when_provided()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.GetParticipants().First();

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, participant.Id,
                "displayname", "new@hmcts.net",
                new List<LinkedParticipantDto>(),
                UserRole.Individual, "Litigant in person")
            {
                Username = "newUser@hmcts.net"
            };
            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedParticipant =
                updatedConference.GetParticipants().Single(x => x.Id == participant.Id);
            updatedParticipant.DisplayName.Should().Be("displayname");
            updatedParticipant.Username.Should().Be(command.Username);

            if (updatedParticipant is Participant updatedParticipantCasted)
            {
                updatedParticipantCasted.ContactEmail.Should().Be("new@hmcts.net");
                updatedParticipantCasted.UserRole.Should().Be(UserRole.Individual);
                updatedParticipantCasted.HearingRole.Should().Be("Litigant in person");
            }
        }

        [Test]
        public async Task Update_participant_details_when_case_hearing_user_role_properties_are_null()
        {
            
            var seededConference = await TestDataManager.SeedConference();
            
            _newConferenceId = seededConference.Id;
            var judge = seededConference.GetParticipants().First(e => e.HearingRole == "Judge");

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, judge.Id,
                "displayname", "new@hmcts.net", new List<LinkedParticipantDto>(),
                UserRole.None, null);
            
            await _handler.Handle(command);
            
            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedJudge =
                updatedConference.GetParticipants().Single(x => x.Id == judge.Id);

            if (updatedJudge is Participant updatedParticipantCasted)
            {
                //userRole assertion
                updatedParticipantCasted.UserRole.Should().NotBe(UserRole.None);
                updatedParticipantCasted.UserRole.Should().Be(judge.UserRole);

                //HearingRole assertion
                updatedParticipantCasted.HearingRole.Should().NotBeNullOrEmpty();
                updatedParticipantCasted.HearingRole.Should().Be(judge.HearingRole);
            }
            else
            {
                Assert.Fail("Participant is not of type Participant");
            }

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
