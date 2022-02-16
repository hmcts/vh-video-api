using System;
using Faker;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.DAL.Commands
{
    public class DomainModelFactoryForTests
    {
        public static Conference CreateConference()
        {
            return new Conference(Guid.NewGuid(), Name.First(), DateTime.UtcNow,
                Name.First(),
                Name.First(), 6, Lorem.GetFirstWord(), false, Name.First());
        }

        public static Participant CreateParticipant()
        {
            return new Participant(Name.First(), Name.First(), Name.First(),
                Name.First(),
                Name.First(), UserRole.Individual, Name.First(), Name.First());
        }

        public static QuickLinkParticipant CreateQuickLinkParticipant(UserRole userRole)
        {
            return new QuickLinkParticipant(Name.First(), userRole);
        }

        public static Participant CreateParticipantWithUsername(string username)
        {
            return new Participant(Name.First(), Name.First(), Name.First(),
                Name.First(),
                username, UserRole.Individual, Name.First(), Name.First());
        }

        public static Participant CreateParticipantCopyForAssertion(Participant participant)
        {
            return new Participant
            {
                Name = participant.Name,
                DisplayName = participant.DisplayName,
                Username = participant.Username,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                ContactEmail = participant.ContactEmail,
                ContactTelephone = participant.ContactTelephone,
                Representee = participant.Representee
            };
        }
    }
}
