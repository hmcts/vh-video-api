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

            var command = new UpdateParticipantDetailsCommand(conferenceId, participantId, "fullname", "firstName",
                "lastName", "displayname", String.Empty, "failed@test.com", "1234");
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_throw_participant_not_found_exception_when_participant_does_not_exist()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participantId = Guid.NewGuid();

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, participantId, "fullname", "firstName",
                "lastName", "displayname", String.Empty, "failed@test.com", "1234");
            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_update_participant_details()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.GetParticipants().First();

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, participant.Id, "fullname", "firstName",
                "lastName", "displayname", String.Empty, "new@test.com", "0123456789");
            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedParticipant =
                updatedConference.GetParticipants().Single(x => x.Username == participant.Username);
            updatedParticipant.DisplayName.Should().Be("displayname");
            updatedParticipant.Name.Should().Be("fullname");
            updatedParticipant.FirstName.Should().Be("firstName");
            updatedParticipant.LastName.Should().Be("lastName");
            updatedParticipant.ContactEmail.Should().Be("new@test.com");
            updatedParticipant.ContactTelephone.Should().Be("0123456789");
        }
        
        [Test]
        public async Task Should_update_participant_username_when_provided()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.GetParticipants().First();

            var command = new UpdateParticipantDetailsCommand(_newConferenceId, participant.Id, "fullname", "firstName",
                "lastName", "displayname", String.Empty, "new@test.com", "0123456789")
            {
                Username = "newUser@updated.com"
            };
            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedParticipant =
                updatedConference.GetParticipants().Single(x => x.Id == participant.Id);
            updatedParticipant.DisplayName.Should().Be("displayname");
            updatedParticipant.Name.Should().Be("fullname");
            updatedParticipant.FirstName.Should().Be("firstName");
            updatedParticipant.LastName.Should().Be("lastName");
            updatedParticipant.ContactEmail.Should().Be("new@test.com");
            updatedParticipant.ContactTelephone.Should().Be("0123456789");
            updatedParticipant.Username.Should().Be(command.Username);
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
