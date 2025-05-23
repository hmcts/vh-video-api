using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class AddEndpointCommandTests : DatabaseTestsBase
    {
        private AddEndpointCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AddEndpointCommandHandler(context);
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
            var command = new AddEndpointCommand(conferenceId, "display", "sip@hmcts.net", "pin", new List<string>() ,ConferenceRole.Host);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task should_add_endpoint_to_conference()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var displayName = "display1";
            var sip = "123@sip.com";
            var pin = "123";
            var defenceAdvocate = seededConference.Participants[0];
            const ConferenceRole conferenceRole = ConferenceRole.Guest;
            
            var command = new AddEndpointCommand(_newConferenceId, displayName, sip, pin, [defenceAdvocate.Username], conferenceRole);
            await _handler.Handle(command);
            
            Conference updatedConference;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                updatedConference = await db.Conferences.Include(x => x.Endpoints).ThenInclude(x => x.ParticipantsLinked).SingleOrDefaultAsync(x => x.Id == _newConferenceId);
            }

            updatedConference.GetEndpoints().Should().NotBeEmpty();
            var ep = updatedConference.Endpoints[0];
            ep.Pin.Should().Be(pin);
            ep.SipAddress.Should().Be(sip);
            ep.DisplayName.Should().Be(displayName);
            ep.Id.Should().NotBeEmpty();
            ep.ParticipantsLinked.Select(e => e.Id).Should().Contain(defenceAdvocate.Id);
            ep.State.Should().Be(EndpointState.NotYetJoined);
            ep.CreatedAt.Should().NotBeNull();
            ep.UpdatedAt.Should().NotBeNull();
        }

        [Test]
        public async Task should_add_endpoint_to_conference_with_no_linked_participants()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var displayName = "display1";
            var sip = "123@sip.com";
            var pin = "123";
            const ConferenceRole conferenceRole = ConferenceRole.Guest;
            
            var command = new AddEndpointCommand(_newConferenceId, displayName, sip, pin, null, conferenceRole);
            await _handler.Handle(command);
            
            Conference updatedConference;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                updatedConference = await db.Conferences.Include(x => x.Endpoints).ThenInclude(x => x.ParticipantsLinked).SingleOrDefaultAsync(x => x.Id == _newConferenceId);
            }

            updatedConference.GetEndpoints().Should().NotBeEmpty();
            var ep = updatedConference.Endpoints[0];
            ep.Pin.Should().Be(pin);
            ep.SipAddress.Should().Be(sip);
            ep.DisplayName.Should().Be(displayName);
            ep.Id.Should().NotBeEmpty();
            ep.ParticipantsLinked.Should().BeEmpty();
            ep.State.Should().Be(EndpointState.NotYetJoined);
            ep.CreatedAt.Should().NotBeNull();
            ep.UpdatedAt.Should().NotBeNull();
        }
    }
}
