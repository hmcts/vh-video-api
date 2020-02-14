using Faker;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class AddMessageCommandTests : DatabaseTestsBase
    {
        private Guid _newConferenceId;
        private AddMessageCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AddMessageCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task should_add_a_message()
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceId = seededConference.Id;

            var participants = seededConference.Participants;
            var from = participants.First(x => x.UserRole == UserRole.Judge).DisplayName;
            var messageText = Internet.DomainWord();

            var command = new AddMessageCommand(_newConferenceId, from, messageText);
            await _handler.Handle(command);

            Conference conference;
            using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                conference = await db.Conferences
                                .Include(x => x.Messages)
                                .SingleAsync(x => x.Id == command.ConferenceId);
            }

            var message = conference.GetMessages().First();

            message.Should().NotBeNull();
            message.From.Should().Be(from);
            message.MessageText.Should().Be(messageText);
            message.TimeStamp.Should().BeBefore(DateTime.UtcNow);
        }

        [Test]
        public void should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var command = new AddMessageCommand(Guid.NewGuid(), "Display Name", "Test message");
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [TearDown]
        public async System.Threading.Tasks.Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
            }
        }
    }
}
