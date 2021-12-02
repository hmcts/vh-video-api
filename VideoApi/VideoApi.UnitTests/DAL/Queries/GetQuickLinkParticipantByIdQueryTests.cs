using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.DAL.Queries
{
    public class GetQuickLinkParticipantByIdQueryTests
    {
        private GetQuickLinkParticipantByIdQueryHandler _handler;
        private VideoApiDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            _dbContext = new VideoApiDbContext(new DbContextOptionsBuilder<VideoApiDbContext>()
                .UseInMemoryDatabase("Test")
                .Options);
            _handler = new GetQuickLinkParticipantByIdQueryHandler(_dbContext);
        }

        [Test]
        public async Task Returns_Conferences_Set_For_Today_With_Specified_Hearing_Venues()
        {
            var quickLinksParticipant = new QuickLinkParticipant("DisplayName", UserRole.QuickLinkParticipant);
            var participant = ParticipantMapper.MapParticipant(quickLinksParticipant);
            
            _dbContext.Participants.Remove(participant);
            await _dbContext.SaveChangesAsync();
            
            await _dbContext.Participants.AddAsync(participant);
            await _dbContext.SaveChangesAsync();
            var result = await _handler.Handle(new GetQuickLinkParticipantByIdQuery { ParticipantId = participant.Id });

            result.DisplayName.Should().Be(quickLinksParticipant.DisplayName);
            result.Id.Should().Be(quickLinksParticipant.Id);
            result.UserRole.Should().Be(quickLinksParticipant.UserRole);
            result.Username.Should().Be(quickLinksParticipant.Username);
            
            _dbContext.Participants.Remove(participant);
            await _dbContext.SaveChangesAsync();
        }

        
    }
}
