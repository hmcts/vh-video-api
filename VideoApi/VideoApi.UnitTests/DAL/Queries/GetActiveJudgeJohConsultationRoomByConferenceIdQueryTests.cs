using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.Contract.Responses;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.DAL.Queries
{
    public class GetActiveJudgeJohConsultationRoomByConferenceIdQueryTests
    {
        private GetActiveJudgeJohConsultationRoomByConferenceIdQueryHandler _handler;
        private VideoApiDbContext _dbContext;
        
        [SetUp]
        public void Setup()
        {
            _dbContext = new VideoApiDbContext(new DbContextOptionsBuilder<VideoApiDbContext>()
                .UseInMemoryDatabase("Test")
                .Options);
            _handler = new GetActiveJudgeJohConsultationRoomByConferenceIdQueryHandler(_dbContext);
        }

        [Test]
        public async Task Returns_Live_JOH_Consultation_Rooms()
        {
            var conferenceId = Guid.NewGuid();
            
            var liveJohRoom = new ConsultationRoom(conferenceId,"RoomLabel",VirtualCourtRoomType.JudgeJOH,true);
            var closedJohRoom = new ConsultationRoom(conferenceId,"RoomLabel",VirtualCourtRoomType.JudgeJOH,true);
            closedJohRoom.CloseRoom();

            _dbContext.Rooms.Add(liveJohRoom);
            _dbContext.Rooms.Add(closedJohRoom);
            await _dbContext.SaveChangesAsync();

            var result = await _handler.Handle(new GetActiveJudgeJohConsultationRoomByConferenceIdQuery(){ConferenceId = conferenceId});

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result.FirstOrDefault().Id.Should().Be(liveJohRoom.Id);
        }
        
        [Test]
        public async Task Returns_Only_Live_JOH_Consultation_Rooms()
        {
            var conferenceId = Guid.NewGuid();
           var liveJohRoom = new ConsultationRoom(conferenceId,"RoomLabel",VirtualCourtRoomType.JudgeJOH,true);
            var liveParticipantRoom = new ConsultationRoom(conferenceId,"ParticipantRoomLabel",VirtualCourtRoomType.Participant,true);
            _dbContext.Rooms.Add(liveJohRoom);
            _dbContext.Rooms.Add(liveParticipantRoom);
            await _dbContext.SaveChangesAsync();
            
            var result = await _handler.Handle(new GetActiveJudgeJohConsultationRoomByConferenceIdQuery(){ConferenceId = conferenceId});
            
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result.FirstOrDefault().Id.Should().Be(liveJohRoom.Id);
        }
        
    }
}
