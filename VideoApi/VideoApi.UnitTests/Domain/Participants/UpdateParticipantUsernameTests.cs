using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Participants
{
    public class UpdateParticipantUsernameTests
    {
        [Test]
        public void Should_update_participant_username()
        {
            // Arrange
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual)
                .WithCaseTypeGroup("Applicant")
                .Build();
            
            var oldUsername = participant.Username;
            const string newUsername = "NewUsername";
            
            // Act
            participant.UpdateUsername(newUsername);

            // Assert
            participant.Username.Should().NotBe(oldUsername);
            participant.Username.Should().Be(newUsername);
        }

        [TestCase(null)]
        [TestCase("")]
        public void Should_throw_exception_when_participant_username_is_null_or_empty(string newUsername)
        {
            // Arrange
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual)
                .WithCaseTypeGroup("Applicant")
                .Build();
            
            var oldUsername = participant.Username;
            
            // Act & assert
            var action = () => participant.UpdateUsername(newUsername);

            action.Should().Throw<DomainRuleException>().Where(x =>
                x.ValidationFailures.Any(v => v.Message == "Username is required"));
            
            participant.Username.Should().NotBe(newUsername);
            participant.Username.Should().Be(oldUsername);
        }
    }
}
