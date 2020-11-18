using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateMeetingRoomCommandTests : DatabaseTestsBase
    {
        private UpdateMeetingRoomHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateMeetingRoomHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public void Should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var command = BuildCommand(conferenceId);

            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_add_conference_virtual_court()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var command = BuildCommand(_newConferenceId);
            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedRoom = updatedConference.GetMeetingRoom();
            updatedRoom.Should().NotBeNull();
            updatedRoom.AdminUri.Should().Be(command.AdminUri);
            updatedRoom.JudgeUri.Should().Be(command.JudgeUri);
            updatedRoom.ParticipantUri.Should().Be(command.ParticipantUri);
            updatedRoom.PexipNode.Should().Be(command.PexipNode);
        }

        [Test]
        public async Task Should_update_conference_virtual_court()
        {
            var conferenceWithVirtualCourt = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithMeetingRoom("https://poc.node.com", "user@email.com").Build();
            var seededConference = await TestDataManager.SeedConference(conferenceWithVirtualCourt);
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var command = BuildCommand(_newConferenceId);
            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedRoom = updatedConference.GetMeetingRoom();
            updatedRoom.Should().NotBeNull();
            updatedRoom.AdminUri.Should().Be(command.AdminUri);
            updatedRoom.JudgeUri.Should().Be(command.JudgeUri);
            updatedRoom.ParticipantUri.Should().Be(command.ParticipantUri);
            updatedRoom.PexipNode.Should().Be(command.PexipNode);
        }

        private UpdateMeetingRoomCommand BuildCommand(Guid conferenceId)
        {
            const string adminUri = "https://testpoc.node.com/viju/#/?conference=user@email.com&output=embed";
            const string judgeUri = "https://testpoc.node.com/viju/#/?conference=user@email.com&output=embed";
            const string participantUri = "https://testpoc.node.com/viju/#/?conference=user@email.com&output=embed";
            const string pexipNode = "testpoc.node.com";
            const int telephoneConferenceId = 12345678;
            return new UpdateMeetingRoomCommand(conferenceId, adminUri, judgeUri, participantUri, pexipNode, telephoneConferenceId);
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
