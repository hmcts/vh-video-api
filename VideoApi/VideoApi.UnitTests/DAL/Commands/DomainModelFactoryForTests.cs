using System;
using Bogus;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.DAL.Commands
{
    public static class DomainModelFactoryForTests
    {
        private static readonly Faker Faker = new();
        public static Conference CreateConference()
        {
            return new Conference(Guid.NewGuid(), Faker.Name.FirstName(), DateTime.UtcNow,
                Faker.Name.FirstName(),
                Faker.Name.FirstName(), 6, Faker.Lorem.Word(), false, Faker.Name.FirstName());
        }

        public static Participant CreateParticipant()
        {
            return new Participant(Faker.Name.FirstName(), Faker.Name.FirstName(),
                UserRole.Individual, Faker.Name.FirstName(), Faker.Name.FirstName());
        }

        public static QuickLinkParticipant CreateQuickLinkParticipant(UserRole userRole)
        {
            return new QuickLinkParticipant(Faker.Name.FirstName(), userRole);
        }

        public static Participant CreateParticipantWithUsername(string username)
        {
            return new Participant(Faker.Name.FirstName(),
                username, UserRole.Individual, Faker.Name.FirstName(), Faker.Name.FirstName());
        }

        public static Participant CreateParticipantCopyForAssertion(Participant participant)
        {
            return new Participant
            {
                DisplayName = participant.DisplayName,
                Username = participant.Username,
                ContactEmail = participant.ContactEmail
            };
        }
        
        public static QuickLinkParticipant CreateQuickLinkParticipantCopyForAssertion(ParticipantBase participant)
        {
            var copyOfParticipant = new QuickLinkParticipant(participant.DisplayName, participant.UserRole);
            copyOfParticipant.DisplayName = participant.DisplayName;
            copyOfParticipant.Username = participant.Username;

            return copyOfParticipant;
        }
    }
}
