using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class CreateConferenceCommandTests : DatabaseTestsBase
    {
        private CreateConferenceCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new CreateConferenceCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task Should_save_new_conference()
        {
            var hearingRefId = Guid.NewGuid();
            const string caseType = "Civil Money Claims";
            var scheduledDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            const string caseNumber = "AutoTest Create Command 1234";
            const string caseName = "AutoTest vs Manual Test";
            const int scheduledDuration = 120;
            var participant = new ParticipantBuilder(true).Build();
            var participants = new List<Participant> {participant};
            const string hearingVenueName = "MyVenue";
            const string ingestUrl = "https://localhost/ingesturl";
            const bool audioRecordingRequired = true;

            var command =
                new CreateConferenceCommand(hearingRefId, caseType, scheduledDateTime, caseNumber, caseName,
                    scheduledDuration, participants, hearingVenueName, audioRecordingRequired, ingestUrl);
            await _handler.Handle(command);

            command.NewConferenceId.Should().NotBeEmpty();
            command.HearingRefId.Should().Be(hearingRefId);
            command.CaseType.Should().Be(caseType);
            command.ScheduledDateTime.Should().Be(scheduledDateTime);
            command.CaseNumber.Should().Be(caseNumber);
            command.CaseName.Should().Be(caseName);
            command.ScheduledDuration.Should().Be(scheduledDuration);
            command.HearingVenueName.Should().Be(hearingVenueName);
            command.AudioRecordingRequired.Should().Be(audioRecordingRequired);
            command.IngestUrl.Should().Be(ingestUrl);
            command.Participants.Should().NotBeNullOrEmpty();
            command.Participants.Count.Should().Be(1);
            command.Participants[0].Should().BeEquivalentTo(participant);
            _newConferenceId = command.NewConferenceId;
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
