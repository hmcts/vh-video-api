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
        public async System.Threading.Tasks.Task should_add_a_message()
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceId = seededConference.Id;

            var participants = seededConference.Participants;
            var from = participants.First(x => x.UserRole == UserRole.Judge).Username;
            var to = participants.First(x => x.UserRole == UserRole.Individual).Username;
            var messageText = Internet.DomainWord();

            var command = new AddMessageCommand(_newConferenceId, from, to, messageText);
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
            message.To.Should().Be(to);
            message.MessageText.Should().Be(messageText);
            message.TimeStamp.Should().BeBefore(DateTime.UtcNow);
        }

        [Test]
        public void should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var command = new AddMessageCommand(Guid.NewGuid(), Internet.Email(), Internet.Email(), Internet.DomainWord());
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async System.Threading.Tasks.Task should_throw_participant_not_found_exception_when_participant_does_not_exist()
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceId = seededConference.Id;

            var from = Internet.Email();
            var to = Internet.Email();
            var messageText = Internet.DomainWord();

            var command = new AddMessageCommand(_newConferenceId, from, to, messageText);
            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _handler.Handle(command));
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
