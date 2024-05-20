using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class RemoveEndpointCommandTests : DatabaseTestsBase
    {
        private RemoveEndpointCommandHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new RemoveEndpointCommandHandler(context);
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
        public void Should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var command = new RemoveEndpointCommand(conferenceId, "sip@sip.com");
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }
        
        [Test]
        public async Task Should_throw_exception_when_endpoint_does_not_exist()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var command = new RemoveEndpointCommand(_newConferenceId, "sip@sip.com");

            Assert.ThrowsAsync<EndpointNotFoundException>(async () =>await _handler.Handle(command));
        }
        
        [Test]
        public async Task Should_remove_existing_endpoint()
        {
            var conference1 = new ConferenceBuilder()
                .WithEndpoint("Display1", "sip@123.com").Build();
            var seededConference = await TestDataManager.SeedConference(conference1);
            var sipAddress = conference1.Endpoints.First().SipAddress;
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            
            var command = new RemoveEndpointCommand(_newConferenceId, sipAddress);
            await _handler.Handle(command);
            
            Conference updatedConference;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                updatedConference = await db.Conferences
                    .Include(x => x.Endpoints)
                    .AsNoTracking().SingleOrDefaultAsync(x => x.Id == _newConferenceId);
            }

            updatedConference.GetEndpoints().Should().BeEmpty();
        }
    }
}
