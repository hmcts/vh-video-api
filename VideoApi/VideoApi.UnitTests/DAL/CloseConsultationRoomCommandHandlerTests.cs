using System;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
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
        public async System.Threading.Tasks.Task Should_close_the_room(VirtualCourtRoomType roomType)
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
        public async System.Threading.Tasks.Task Should_throw_invalid_room_type_exception(VirtualCourtRoomType roomType)
        {
            // Arrange
            const string roomLabel = "label";
            ConsultationRoom room = new ConsultationRoom(Guid.Empty, roomLabel, roomType, false);
            await _dbContext.Rooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();
            
            // Act & Assert
            Func<Task> action = async () => await _handlerUnderTest.Handle(new CloseConsultationRoomCommand(room.Id));
            await action.Should().ThrowAsync<InvalidVirtualCourtRoomTypeException>();
        }
    }
}
