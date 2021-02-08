using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Queries;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetEndpointsForConferenceQueryTests : DatabaseTestsBase
    {
        private GetEndpointsForConferenceQueryHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetEndpointsForConferenceQueryHandler(context);
            _newConferenceId = Guid.Empty;
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
        
        [Test]
        public async Task should_return_empty_list_of_endpoints()
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceId = seededConference.Id;
            var query = new GetEndpointsForConferenceQuery(_newConferenceId);

            var result = await _handler.Handle(query);
            result.Should().BeEmpty();
        }

        [Test]
        public async Task should_return_list_of_endpoints()
        {
            var conference1 = new ConferenceBuilder()
                .WithEndpoint("Display1", "sip@123.com")
                .WithEndpoint("Display2", "sip@321.com").Build();

            _newConferenceId = conference1.Id;
            await TestDataManager.SeedConference(conference1);
            
            var query = new GetEndpointsForConferenceQuery(_newConferenceId);

            var result = await _handler.Handle(query);
            result.Should().BeEquivalentTo(conference1.GetEndpoints());
        }

        [Test]
        public async Task should_return_empty_list_of_endpoints_when_conference_does_not_exist()
        {
            var query = new GetEndpointsForConferenceQuery(Guid.NewGuid());
            var result = await _handler.Handle(query);
            result.Should().BeEmpty();
        }
    }
}
