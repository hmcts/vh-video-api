using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class RemoveInstantMessagesForConferenceCommandTests : DatabaseTestsBase
    {
        private RemoveMessagesForConferenceCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new RemoveMessagesForConferenceCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

       
        [Test]
        public async Task Should_remove_messages_for_conference()
        {
            var conference = new ConferenceBuilder(true)
               .WithMessages(2)
               .Build();

            var seededConference = await TestDataManager.SeedConference(conference);
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
                       
            var command = new RemoveInstantMessagesForConferenceCommand(_newConferenceId);
            await _handler.Handle(command);

            List<InstantMessage> messages;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                messages = await db.InstantMessages.AsQueryable().Where(x => x.ConferenceId == command.ConferenceId).ToListAsync();
            }

            var afterCount = messages.Count;
            afterCount.Should().Be(0);
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
