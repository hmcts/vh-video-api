using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Queries;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetConferenceForParticipantQueryTests : DatabaseTestsBase
    {
        private GetConferenceForParticipantQueryHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void SetUp()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetConferenceForParticipantQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }
        
        [Test]
        public async Task Should_get_conference_details_for_participant()
        {
            // Arrange
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.Participants[0];
            
            // Act
            var conference = await _handler.Handle(new GetConferenceForParticipantQuery(participant.Id));

            // Assert
            conference.Should().NotBeNull();
            conference.CaseType.Should().Be(seededConference.CaseType);
            conference.CaseNumber.Should().Be(seededConference.CaseNumber);
            conference.ScheduledDuration.Should().Be(seededConference.ScheduledDuration);
            conference.ScheduledDateTime.Should().Be(seededConference.ScheduledDateTime);
            conference.HearingRefId.Should().Be(seededConference.HearingRefId);
            conference.Supplier.Should().Be(seededConference.Supplier);
        }

        [Test]
        public async Task Should_return_null_when_participant_not_found()
        {
            // Arrange
            var participantId = Guid.NewGuid();
            
            // Act
            var conference = await _handler.Handle(new GetConferenceForParticipantQuery(participantId));
            
            // Assert
            conference.Should().BeNull();
        }
        
        [TearDown]
        public async Task TearDown()
        {
            await TestDataManager.RemoveConference(_newConferenceId);
        }
    }
}
