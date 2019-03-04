using System;
using System.Linq;
using Faker;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders;
using VideoApi.Domain;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class AddParticipantTests
    {
        [Test]
        public void should_add_new_participant_to_hearing()
        {
            var conference = new ConferenceBuilder().Build();

            var beforeCount = conference.GetParticipants().Count;
            var participant = new ParticipantBuilder().Build();
            conference.AddParticipant(participant);

            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().BeGreaterThan(beforeCount);
        }

        [Test]
        public void should_not_add_existing_participant_to_hearing()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant("Claimant LIP", "Claimant")
                .WithParticipant("Solicitor", "Claimant")
                .WithParticipant("Solicitor LIP", "Defendant")
                .WithParticipant("Solicitor", "Defendant")
                .Build();

            var beforeCount = conference.GetParticipants().Count;

            var participant = conference.GetParticipants().First();

            Action action = () => conference.AddParticipant(participant);

            action.Should().Throw<DomainRuleException>();
            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().Be(beforeCount);
        }
    }
}