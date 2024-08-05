using Microsoft.EntityFrameworkCore;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.DAL.Queries;

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
    public async Task Returns_quick_link_participant_when_query_by_Id()
    {
        var quickLinksParticipant = new QuickLinkParticipant("DisplayName", UserRole.QuickLinkParticipant);
        var participant = new Participant
        {
            ParticipantRefId = quickLinksParticipant.Id,
            Name = quickLinksParticipant.Name,
            DisplayName = quickLinksParticipant.DisplayName,
            Username = quickLinksParticipant.Username,
            UserRole = quickLinksParticipant.UserRole,
            State = quickLinksParticipant.State,
        };
        
        var isExisting = await _handler.Handle(new GetQuickLinkParticipantByIdQuery { ParticipantId = participant.Id }) != null;
        
        if (isExisting)
        {
            _dbContext.Participants.Remove(participant);
            await _dbContext.SaveChangesAsync();
        }
        
        await _dbContext.Participants.AddAsync(participant);
        await _dbContext.SaveChangesAsync();
        var result = await _handler.Handle(new GetQuickLinkParticipantByIdQuery { ParticipantId = participant.Id });
        
        result.DisplayName.Should().Be(quickLinksParticipant.DisplayName);
        result.UserRole.Should().Be(quickLinksParticipant.UserRole);
        result.Username.Should().Be(quickLinksParticipant.Username);
        
        _dbContext.Participants.Remove(participant);
        await _dbContext.SaveChangesAsync();
    }
}
