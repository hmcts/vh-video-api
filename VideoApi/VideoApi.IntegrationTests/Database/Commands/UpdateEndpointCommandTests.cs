using System;
using System.Linq;
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
    public class UpdateEndpointCommandTests : DatabaseTestsBase
    {
        private UpdateEndpointCommandHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateEndpointCommandHandler(context);
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
            var displayName = "new endpoint";
            var command = new UpdateEndpointCommand(conferenceId, "sip@sip.com", displayName, null,
                ConferenceRole.Host);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }
        
        [Test]
        public async Task Should_throw_exception_when_endpoint_does_not_exist()
        {
            var seededConference = await TestDataManager.SeedConference();
            var displayName = "new endpoint";
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var command = new UpdateEndpointCommand(_newConferenceId, "sip@sip.com", displayName, null,
                ConferenceRole.Host);

            Assert.ThrowsAsync<EndpointNotFoundException>(async () => await _handler.Handle(command));
        }
        
        [Test]
        public async Task Should_update_existing_endpoint_with_new_display_name()
        {
            var conference1 = new ConferenceBuilder()
                .WithEndpoint("DisplayName", "sip@12356.com").Build();
            var seededConference = await TestDataManager.SeedConference(conference1);
            var ep = conference1.Endpoints.First();
            var sipAddress = ep.SipAddress;
            var newDisplayName = "Alternate Display Name";
            const ConferenceRole newConferenceRole = ConferenceRole.Guest;
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var command = new UpdateEndpointCommand(_newConferenceId, sipAddress, newDisplayName, null, newConferenceRole);
            await _handler.Handle(command);

            Conference updatedConference;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                updatedConference = await db.Conferences.Include(x => x.Endpoints)
                    .AsNoTracking().SingleAsync(x => x.Id == _newConferenceId);
            }
            
            var updatedEndpoint = updatedConference.GetEndpoints().Single(x => x.SipAddress == sipAddress);
            updatedEndpoint.DisplayName.Should().Be(newDisplayName);
            updatedEndpoint.ConferenceRole.Should().Be(newConferenceRole);
            
            ep.CreatedAt.Should().Be(updatedEndpoint.CreatedAt);
            updatedEndpoint.UpdatedAt.Should().BeAfter(updatedEndpoint.CreatedAt.Value);
        }
        
        [Test]
        public async Task Should_update_existing_endpoint_with_linked_participant()
        {
            var conference1 = new ConferenceBuilder()
                .WithParticipant(UserRole.Representative, "CaseTypeGroupe")
                .WithEndpoint("DisplayName", "sip@1236546.com").Build();
            var seededConference = await TestDataManager.SeedConference(conference1);
            var ep = conference1.Endpoints.First();
            var sipAddress = ep.SipAddress;
            var defenceAdvocate = conference1.Participants[0].Username;
            const ConferenceRole newConferenceRole = ConferenceRole.Guest;
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var command = new UpdateEndpointCommand(_newConferenceId, sipAddress, null, [defenceAdvocate], newConferenceRole);
            await _handler.Handle(command);

            Conference updatedConference;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                updatedConference = await db.Conferences
                    .Include(x => x.Endpoints).ThenInclude(x => x.ParticipantsLinked)
                    .AsNoTracking().SingleAsync(x => x.Id == _newConferenceId);
            }
            
            var updatedEndpoint = updatedConference.GetEndpoints().Single(x => x.SipAddress == sipAddress);
            updatedEndpoint.DisplayName.Should().Be(ep.DisplayName);
            updatedEndpoint.ParticipantsLinked.Should().Contain(e => e.Username == defenceAdvocate);
            updatedEndpoint.ConferenceRole.Should().Be(newConferenceRole);
        }
    }
}
