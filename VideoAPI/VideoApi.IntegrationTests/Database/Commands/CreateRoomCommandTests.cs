using FluentAssertions;
using NUnit.Framework;
using System;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class CreateRoomCommandTests : DatabaseTestsBase
    {
        private CreateRoomCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new CreateRoomCommandHandler(context);
        }


        [Test]
        public async Task Should_save_new_room()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;


            var command = new CreateRoomCommand(_newConferenceId, "Room1", VirtualCourtRoomType.JudgeJOH);

            await _handler.Handle(command);

            command.NewRoomId.Should().BeGreaterThan(0);
            command.ConferenceId.Should().Be(_newConferenceId);
            command.Type.Should().Be(VirtualCourtRoomType.JudgeJOH);
        }


        [Test]
        public void Should_throw_exception_if_no_conference_found()
        {
            var command = new CreateRoomCommand(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);
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
