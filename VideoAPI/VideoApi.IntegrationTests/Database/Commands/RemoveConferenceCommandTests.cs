using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class RemoveConferenceCommandTests : DatabaseTestsBase
    {
        private RemoveConferenceCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new RemoveConferenceCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public void Should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();

            var command = new RemoveConferenceCommand(conferenceId);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_remove_conference()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var command = new RemoveConferenceCommand(_newConferenceId);
            await _handler.Handle(command);

            Conference removedConference;
            using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                removedConference = await db.Conferences.SingleOrDefaultAsync(x => x.Id == _newConferenceId);
            }

            removedConference.Should().BeNull();
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
    }
}