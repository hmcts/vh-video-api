using System;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetConferenceByHearingRefIdQueryTests : DatabaseTestsBase
    {
        private GetConferenceByHearingRefIdQueryHandler _handler;
        private Guid _newConferenceId1;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetConferenceByHearingRefIdQueryHandler(context);
            _newConferenceId1 = Guid.Empty;
        }

        [Test]
        public async Task Should_get_conference_details_by_hearing_ref_id()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId1 = seededConference.Id;

            var conference = await _handler.Handle(new GetConferenceByHearingRefIdQuery(seededConference.HearingRefId));

            conference.Should().NotBeNull();

            conference.CaseType.Should().Be(seededConference.CaseType);
            conference.CaseNumber.Should().Be(seededConference.CaseNumber);
            conference.ScheduledDuration.Should().Be(seededConference.ScheduledDuration);
            conference.ScheduledDateTime.Should().Be(seededConference.ScheduledDateTime);
            conference.HearingRefId.Should().Be(seededConference.HearingRefId);
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestDataManager.RemoveConference(_newConferenceId1);
        }
    }
}
