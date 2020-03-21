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
                .Any(x => x.Message == "Cannot set conference status to 'none'").Should().BeTrue();
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
            conference.ClosedDateTime.Value.Should().BeAfter(beforeActionTime);
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
    }
}