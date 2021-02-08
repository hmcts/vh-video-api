using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class UpdateConferenceStatusTests
    {
        [Test]
        public void Should_throw_exception_when_updating_with_invalid_state()
        {
            var conference = new ConferenceBuilder()
                .Build();

            Action action = () => conference.UpdateConferenceStatus(ConferenceState.NotStarted);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Any(x => x.Message == "Cannot set conference status to 'Not Started'").Should().BeTrue();
        }

        [Test]
        public void Should_update_close_time_when_updating_status_to_closed()
        {
            var beforeActionTime = DateTime.UtcNow;
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Claimant")
                .Build();
            
            conference.GetCurrentStatus().Should().Be(ConferenceState.NotStarted);
            conference.ClosedDateTime.Should().BeNull();
            conference.UpdateConferenceStatus(ConferenceState.Closed);
            conference.ClosedDateTime.Should().NotBeNull();
            conference.ClosedDateTime.Should().HaveValue().And.BeAfter(beforeActionTime);
            conference.GetCurrentStatus().Should().Be(ConferenceState.Closed);
        }

        [Test]
        public void Should_add_conference_status()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Claimant")
                .Build();

            conference.GetCurrentStatus().Should().Be(ConferenceState.NotStarted);
            var beforeCount = conference.GetConferenceStatuses().Count;

            conference.UpdateConferenceStatus(ConferenceState.InSession);
            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().BeGreaterThan(beforeCount);

            conference.GetCurrentStatus().Should().Be(ConferenceState.InSession);
        }

        [TestCase(ConferenceState.InSession, true)]
        [TestCase(ConferenceState.Paused, false)]
        [TestCase(ConferenceState.Suspended, false)]
        [TestCase(ConferenceState.Closed, false)]
        public void should_set_start_time_when_status_changes_to_in_session_first_time(ConferenceState status,
            bool startTimeSet)
        {
            var conference = new ConferenceBuilder().Build();
            conference.UpdateConferenceStatus(status);
            conference.ActualStartTime.HasValue.Should().Be(startTimeSet);
        }

        [Test]
        public void should_not_reset_start_time_on_resume()
        {
            var conferenceType = typeof(VideoApi.Domain.Conference);
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession).WithConferenceStatus(ConferenceState.Paused)
                .Build();
            var existingStartTime = DateTime.UtcNow.AddMinutes(-5); 
            conferenceType.GetProperty(nameof(conference.ActualStartTime))?.SetValue(conference, existingStartTime);
            
            conference.UpdateConferenceStatus(ConferenceState.InSession);
            
            conference.ActualStartTime.Should().Be(existingStartTime);
        }
    }
}
