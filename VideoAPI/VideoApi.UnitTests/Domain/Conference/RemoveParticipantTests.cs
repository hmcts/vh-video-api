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
    public class RemoveParticipantTests
    {
        [Test]
        public void should_remove_participant_from_hearing()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant("Claimant LIP", "Claimant")
                .Build();

            var beforeCount = conference.GetParticipants().Count;

            var participant = conference.GetParticipants().First();

            conference.RemoveParticipant(participant);

            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().BeLessThan(beforeCount);
        }

        [Test]
        public void should_not_fail_when_removing_non_existent_participant()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant("Claimant LIP", "Claimant")
                .Build();

            var beforeCount = conference.GetParticipants().Count;
            var participant = Builder<Participant>.CreateNew().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), Name.First(), Internet.Email(), "Solicitor",
                    "Claimant")).Build();

            Action action = () => conference.RemoveParticipant(participant);

            action.Should().Throw<DomainRuleException>();
            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().Be(beforeCount);
        }
    }
}