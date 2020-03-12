using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using System.Collections.Generic;
using System.Linq;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;
using Testing.Common.Helper.Builders.Domain;
using Microsoft.EntityFrameworkCore;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class RemoveInstantMessagesForConferenceCommandTests : DatabaseTestsBase
    {
        private RemoveMessagesForConferenceCommandHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new RemoveMessagesForConferenceCommandHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public void should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var command = new RemoveInstantMessagesForConferenceCommand(conferenceId);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task should_remove_messages_from_conference()
        {
            var conference = new ConferenceBuilder(true)
               .WithMessages(2)
               .Build();

            var seededConference = await TestDataManager.SeedConference(conference);
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
                       
            var command = new RemoveInstantMessagesForConferenceCommand(_newConferenceId);
            await _handler.Handle(command);

            Conference updatedConference;
            using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                updatedConference = await db.Conferences
                                .Include(x => x.InstantMessageHistory)
                                .SingleAsync(x => x.Id == command.ConferenceId);
            }

            var afterCount = updatedConference.GetInstantMessageHistory().Count;
            afterCount.Should().Equals(0);
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
