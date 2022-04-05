using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateConferenceDetailsCommandTests : DatabaseTestsBase
    {
        private UpdateConferenceDetailsCommandHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        private Guid _conferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateConferenceDetailsCommandHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
            _conferenceId = Guid.Empty;
        }

        [Test]
        public void Should_throw_exception_when_conference_does_not_exist()
        {
            var hearingRefId = Guid.NewGuid();
            var command = new UpdateConferenceDetailsCommand(hearingRefId, "caseNo", "caseType", 
                "caseName", 10, DateTime.Today, "MyVenue", false);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_update_conference_details()
        {
            var seededConference = await TestDataManager.SeedConference();
            _conferenceId = seededConference.Id;
            var hearingRefId = seededConference.HearingRefId;
            var duration = seededConference.ScheduledDuration + 10;
            var scheduledDateTime = seededConference.ScheduledDateTime.AddDays(1);

            var command = new UpdateConferenceDetailsCommand(hearingRefId, seededConference.CaseNumber,
                seededConference.CaseType, seededConference.CaseName, duration, scheduledDateTime, "MyVenue", true);

            await _handler.Handle(command);
            
            var conference = _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(seededConference.Id)).Result;
            conference.ScheduledDuration.Should().Be(duration);
            conference.ScheduledDateTime.Should().Be(scheduledDateTime);
            conference.ScheduledDateTime.Should().Be(scheduledDateTime);

            seededConference.AudioRecordingRequired.Should().BeFalse();
            conference.AudioRecordingRequired.Should().BeTrue();
            seededConference.CreatedDateTime.Should().Be(conference.CreatedDateTime);
            conference.UpdatedAt.Should().BeAfter(conference.CreatedDateTime.Value);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_conferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_conferenceId}");
                await TestDataManager.RemoveConference(_conferenceId);
            }
        }
    }
}
