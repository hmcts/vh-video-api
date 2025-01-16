using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class AddMessageCommandTests : DatabaseTestsBase
    {
        private Guid _newConferenceId;
        private AddInstantMessageCommandHandler _handler;

        private static readonly Faker Faker = new();

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AddInstantMessageCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task Should_add_a_message()
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceId = seededConference.Id;

            var participants = seededConference.Participants;
            var from = participants.First(x => x.UserRole == UserRole.Judge).DisplayName;
            var messageText = Faker.Internet.DomainWord();
            var to = "VH Officer";

            var command = new AddInstantMessageCommand(_newConferenceId, from, messageText, to);
            await _handler.Handle(command);

            List<InstantMessage> instantMessages;
            using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                instantMessages = await db.InstantMessages
                                .Where(x => x.ConferenceId == command.ConferenceId).ToListAsync();
            }

            var message = instantMessages.First();

            message.Should().NotBeNull();
            message.From.Should().Be(from);
            message.MessageText.Should().Be(messageText);
            message.TimeStamp.Should().BeBefore(DateTime.UtcNow);
            message.ConferenceId.Should().Be(command.ConferenceId);
            message.To.Should().Be(to);
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
