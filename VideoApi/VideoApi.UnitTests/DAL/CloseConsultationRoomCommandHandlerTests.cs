using System;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.DAL
{
    [TestFixture]
    public class CloseConsultationRoomCommandHandlerTests
    {
        private CloseConsultationRoomCommandHandler _handlerUnderTest;
        private VideoApiDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            _dbContext = new VideoApiDbContext(new DbContextOptionsBuilder<VideoApiDbContext>()
                .UseInMemoryDatabase("Test")
                .Options);
            _handlerUnderTest = new CloseConsultationRoomCommandHandler(_dbContext);
        }

        [TestCase(VirtualCourtRoomType.Participant)]
        [TestCase(VirtualCourtRoomType.JudgeJOH)]
        public async Task Should_close_the_room(VirtualCourtRoomType roomType)
        {
            // Arrange
            const string roomLabel = "label";
            ConsultationRoom room = new ConsultationRoom(Guid.Empty, roomLabel, roomType, false);
            await _dbContext.Rooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();
            
            // Act
            await _handlerUnderTest.Handle(new CloseConsultationRoomCommand(room.Id));
            
            // Assert
            var dbRoom = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == room.Id);
            dbRoom?.Status.Should().Be(RoomStatus.Closed);
        }
        
        [TestCase(VirtualCourtRoomType.WaitingRoom)]
        [TestCase(VirtualCourtRoomType.JudicialShared)]
        [TestCase(VirtualCourtRoomType.Witness)]
        [TestCase(VirtualCourtRoomType.Civilian)]
        public async Task Should_throw_invalid_room_type_exception(VirtualCourtRoomType roomType)
        {
            // Arrange
            const string roomLabel = "label";
            ConsultationRoom room = new ConsultationRoom(Guid.Empty, roomLabel, roomType, false);
            await _dbContext.Rooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();
            
            // Act & Assert
            Func<Task> act = async () => await _handlerUnderTest.Handle(new CloseConsultationRoomCommand(room.Id));
            await act.Should().ThrowAsync<InvalidVirtualCourtRoomTypeException>();
        }
        
        [Test]
        public async Task Should_throw_room_not_found_exception()
        {
            // Arrange
            const long roomId = 42;
            
            // Act & Assert
            Func<Task> act = async () => await _handlerUnderTest.Handle(new CloseConsultationRoomCommand(roomId));
            (await act.Should().ThrowAsync<RoomNotFoundException>()).And.Message.Should().Contain($"Room '{roomId}' not found");
        }
    }
}
