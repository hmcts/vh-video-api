using System;
using System.Linq;
using FluentAssertions;
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
    public class UpdateSelfTestCallResultCommandHandlerTests : DatabaseTestsBase
    {
        private UpdateSelfTestCallResultCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateSelfTestCallResultCommandHandler(context);
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
            var participantId = Guid.NewGuid();
            var command = new UpdateSelfTestCallResultCommand(conferenceId, participantId, true, TestScore.Good);

            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_throw_participant_not_found_exception_when_participant_does_not_exist()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participantId = Guid.NewGuid();
            var command = new UpdateSelfTestCallResultCommand(_newConferenceId, participantId, true, TestScore.Good);

            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _handler.Handle(command));
        }
        
        [Test]
        public async Task Should_update_participant_self_test_score()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participantId = seededConference.Participants.First().Id;
            var command = new UpdateSelfTestCallResultCommand(_newConferenceId, participantId, true, TestScore.Good);

            await _handler.Handle(command);

            Conference resultConference;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                resultConference = await db.Conferences.Include(x => x.Participants).ThenInclude(x => x.TestCallResult)
                    .SingleAsync(x => x.Id == command.ConferenceId);
            }

            var resultParticipant = resultConference.GetParticipants().Single(x => x.Id == participantId);
            resultParticipant.TestCallResult.Passed.Should().BeTrue();
            resultParticipant.TestCallResult.Score.Should().Be(TestScore.Good);

            resultParticipant.TestCallResult.Timestamp.Should().BeBefore(DateTime.UtcNow);
        }
    }
}
