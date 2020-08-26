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
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateEndpointStatusCommandTests : DatabaseTestsBase
    {
        private UpdateEndpointStatusCommandHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateEndpointStatusCommandHandler(context);
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
            var endpointId = Guid.NewGuid();
            const EndpointState status = EndpointState.Connected;
            var command = new UpdateEndpointStatusCommand(conferenceId, endpointId, status);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_throw_exception_when_endpoint_does_not_exist()
        {
            var seededConference = await TestDataManager.SeedConference();
            var endpointId = Guid.NewGuid();
            const EndpointState status = EndpointState.Connected;
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var command = new UpdateEndpointStatusCommand(_newConferenceId, endpointId, status);

            Assert.ThrowsAsync<EndpointNotFoundException>(async () => await _handler.Handle(command));
        }

        [Test]
        public async Task Should_update_existing_endpoint_with_new_status()
        {
            var conference1 = new ConferenceBuilder()
                .WithEndpoint("DisplayName", "sip@123.com").Build();
            var seededConference = await TestDataManager.SeedConference(conference1);
            var endpointId = conference1.Endpoints.First().Id;
            const EndpointState status = EndpointState.Connected;
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var command = new UpdateEndpointStatusCommand(_newConferenceId, endpointId, status);
            await _handler.Handle(command);

            Conference updatedConference;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                updatedConference = await db.Conferences.Include(x => x.Endpoints)
                    .AsNoTracking().SingleOrDefaultAsync(x => x.Id == _newConferenceId);
            }
            var updatedEndpoint = updatedConference.GetEndpoints().Single(x => x.Id == endpointId);
            updatedEndpoint.State.Should().Be(status);
        }
    }
}
