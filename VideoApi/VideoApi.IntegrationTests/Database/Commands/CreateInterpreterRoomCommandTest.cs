using System;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class CreateInterpreterRoomCommandTest : DatabaseTestsBase
    {
        private CreateParticipantRoomCommandHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new CreateParticipantRoomCommandHandler(context);
        }
        
        [Test]
        public async Task Should_save_new_room()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;


            var command = new CreateParticipantRoomCommand(_newConferenceId,  VirtualCourtRoomType.Civilian);

            await _handler.Handle(command);

            command.NewRoomId.Should().BeGreaterThan(0);
            command.ConferenceId.Should().Be(_newConferenceId);
            command.Type.Should().Be(command.Type);
        }

        [Test]
        public void Should_throw_exception_if_no_conference_found()
        {
            var command = new CreateParticipantRoomCommand(Guid.NewGuid(), VirtualCourtRoomType.Civilian);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }
        
        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test room for conference {_newConferenceId}");
                await TestDataManager.RemoveRooms(_newConferenceId);
            
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
                _newConferenceId = Guid.Empty;
            }
            
        }
    }
}
