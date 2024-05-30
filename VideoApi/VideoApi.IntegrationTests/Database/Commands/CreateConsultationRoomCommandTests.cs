using NUnit.Framework;
using System;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class CreateConsultationRoomCommandTests : DatabaseTestsBase
    {
        private CreateConsultationRoomCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new CreateConsultationRoomCommandHandler(context);
        }

        [Test]
        public async Task Should_save_new_room()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;


            var command = new CreateConsultationRoomCommand(_newConferenceId, "Room1", VirtualCourtRoomType.JudgeJOH, false);

            await _handler.Handle(command);
            
            command.NewRoomId.Should().BeGreaterThan(0);
            command.ConferenceId.Should().Be(_newConferenceId);
            command.Type.Should().Be(VirtualCourtRoomType.JudgeJOH);
            
            await using var db = new VideoApiDbContext(VideoBookingsDbContextOptions);
            var updatedConference = await db.Conferences.Include(x => x.Rooms).FirstAsync(x => x.Id == seededConference.Id);
            updatedConference.Rooms.Should().Contain(x => x.Label == "Room1");
        }

        [Test]
        public void Should_throw_exception_if_no_conference_found()
        {
            var command = new CreateConsultationRoomCommand(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH, false);
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
