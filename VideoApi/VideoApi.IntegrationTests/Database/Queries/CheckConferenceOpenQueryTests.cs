using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Queries;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class CheckConferenceOpenQueryTests : DatabaseTestsBase
    {
        private CheckConferenceOpenQueryHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new CheckConferenceOpenQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task Should_return_null_when_conference_not_already_booked()
        {
            var query = new CheckConferenceOpenQuery(DateTime.UtcNow, "12345678HBGH", "New Case", Guid.NewGuid());

            var conference = await _handler.Handle(query);

            conference.Should().BeNull();
        }
        
        [Test]
        public async Task Should_return_conference_when_query_is_matched()
        {
            var seededConference = await TestDataManager.SeedConference();

            var query = new CheckConferenceOpenQuery(seededConference.ScheduledDateTime, seededConference.CaseNumber,
                seededConference.CaseName, seededConference.HearingRefId);

            var conference = await _handler.Handle(query);

            conference.Should().NotBeNull();
            conference.Id.Should().Be(seededConference.Id);
            _newConferenceId = seededConference.Id;
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
            }
        }
    }
}
