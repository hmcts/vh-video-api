using System;
using System.Linq;
using Bogus;
using FizzWare.NBuilder;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class RemoveParticipantTests
    {
        private static readonly Faker Faker = new();
        
        [Test]
        public void Should_remove_participant_from_hearing()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Applicant")
                .Build();

            var beforeCount = conference.GetParticipants().Count;

            var participant = conference.GetParticipants().First();

            conference.RemoveParticipant(participant);

            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().BeLessThan(beforeCount);
        }

        [Test]
        public void Should_not_fail_when_removing_non_existent_participant()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Applicant")
                .Build();

            var beforeCount = conference.GetParticipants().Count;
            var userRole = UserRole.Representative;
            var caseGroup = "Applicant";
            var hearingRole = ParticipantBuilder.DetermineHearingRole(userRole, caseGroup);
            var participant = Builder<Participant>.CreateNew().WithFactory(() =>
                new Participant(Guid.NewGuid(), Faker.Name.FullName(),
                    $"{Faker.Random.Number(0, 99999999)}@hmcts.net", userRole, hearingRole, $"{Faker.Random.Number(0, 99999999)}@hmcts.net")).Build();

            Action action = () => conference.RemoveParticipant(participant);

            action.Should().Throw<DomainRuleException>();
            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().Be(beforeCount);
        }
    }
}
