using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateInterpreterRoomConnectionDetailsCommandTests :DatabaseTestsBase
    {
        private UpdateInterpreterRoomConnectionDetailsCommandHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateInterpreterRoomConnectionDetailsCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public void should_throw_exception_if_room_does_not_exist()
        {
            var command = new UpdateInterpreterRoomConnectionDetailsCommand(Guid.NewGuid(), int.MaxValue, "Label", "ingest", "node", "uri");
            Assert.ThrowsAsync<RoomNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task should_update_room_with_new_details()
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceId = seededConference.Id;
            var room = new InterpreterRoom(_newConferenceId, VirtualCourtRoomType.Civilian);
            await TestDataManager.SeedRooms(new []{room});

            var label = "Interpreter1";
            var ingestUrl = "dummyurl";
            var node = "node";
            var uri = "fakeuri";
            var command = new UpdateInterpreterRoomConnectionDetailsCommand(_newConferenceId, room.Id, label, ingestUrl, node, uri);
            await _handler.Handle(command);
            var roomId = command.RoomId;

            await using var db = new VideoApiDbContext(VideoBookingsDbContextOptions);
            var updatedRoom = await db.Rooms.OfType<InterpreterRoom>().AsNoTracking()
                .SingleAsync(c => c.Id == roomId);
            updatedRoom.Label.Should().Be(label);
            updatedRoom.IngestUrl.Should().Be(ingestUrl);
            updatedRoom.PexipNode.Should().Be(node);
            updatedRoom.ParticipantUri.Should().Be(uri);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                await TestDataManager.RemoveRooms(_newConferenceId);
            }
        }
    }
}
