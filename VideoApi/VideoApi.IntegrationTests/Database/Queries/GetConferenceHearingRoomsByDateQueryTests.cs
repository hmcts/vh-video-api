using NUnit.Framework;
using System;
using System.Threading.Tasks;
using VideoApi.DAL;
using VideoApi.DAL.Queries;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetConferenceHearingRoomsByDateQueryTests : DatabaseTestsBase
    {
        private GetConferenceHearingRoomsByDateQueryHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            _newConferenceId = Guid.Empty;
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetConferenceHearingRoomsByDateQueryHandler(context);
        }

        [Test]
        public async Task Should_return_an_empty_list_if_conference_does_not_exist()
        {
            var query = new GetConferenceHearingRoomsByDateQuery(DateTime.Today.AddDays(1));
            var result = await _handler.Handle(query);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task Should_return_list_if_conference_status_exist()
        {
            var conference = await TestDataManager.SeedConference(true);
            _newConferenceId = conference.Id;

            var query = new GetConferenceHearingRoomsByDateQuery(DateTime.Today);
            var result = await _handler.Handle(query);

            result.Should().NotBeEmpty();
            result[0].HearingRefId.Should().Be(conference.HearingRefId);
            result[0].Label.Should().BeEmpty();
            result[0].FileNamePrefix.Should().Be($"{conference.IngestUrlFilenamePrefix}_{DateTime.Today:yyyy-MM-dd}");
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine("Cleaning conferences for GetInterpreterRoomsForConferenceQuery");
                await TestDataManager.RemoveConference(_newConferenceId);
            }
        }

    }
}
