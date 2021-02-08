using Autofac.Extras.Moq;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using VideoApi.Common.Helpers;
using VideoApi.Domain;

namespace VideoApi.UnitTests.Helpers
{
    [TestFixture]
    public class LoggingDataExtractorTests
    {
        private AutoMock _mocker;

        private LoggingDataExtractor _sut;

        [SetUp]
        public void ExceptionMiddleWareSetup()
        {
            _mocker = AutoMock.GetLoose();
            _sut = _mocker.Create<LoggingDataExtractor>();
        }

        [Test]
        public void ConferenceShouldConvertToDictionary()
        {
            // Arrange
            var conference = new Conference(Guid.NewGuid(), "CaseType", DateTime.UtcNow, "CaseNumber", "CaseName", 10, "HearingVenueName", false, "IngestUrl");

            // Act
            var result = _sut.ConvertToDictionary(conference);

            // Assert
            result.Should().Contain(new KeyValuePair<string, object>(nameof(conference.HearingRefId), conference.HearingRefId));
            result.Should().Contain(new KeyValuePair<string, object>(nameof(conference.CaseType), conference.CaseType));
            result.Should().Contain(new KeyValuePair<string, object>(nameof(conference.ScheduledDateTime), conference.ScheduledDateTime));
            result.Should().Contain(new KeyValuePair<string, object>(nameof(conference.CaseNumber), conference.CaseNumber));
            result.Should().Contain(new KeyValuePair<string, object>(nameof(conference.CaseName), conference.CaseName));
            result.Should().Contain(new KeyValuePair<string, object>(nameof(conference.ScheduledDuration), conference.ScheduledDuration));
            result.Should().Contain(new KeyValuePair<string, object>(nameof(conference.AudioRecordingRequired), conference.AudioRecordingRequired));
            result.Should().Contain(new KeyValuePair<string, object>(nameof(conference.IngestUrl), conference.IngestUrl));
            result.Should().Contain(new KeyValuePair<string, object>($"{nameof(conference.MeetingRoom)}.{nameof(conference.MeetingRoom.AdminUri)}", conference.MeetingRoom.AdminUri));
        }
    }
}
