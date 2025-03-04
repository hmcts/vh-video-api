using System;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain
{
    public class ParticipantTests
    {
        private Guid _testParticipantRefId;
        private string _testDisplayName;
        private string _testUsername;
        private UserRole _testUserRole;
        private string _testHearingRole;
        private string _testContactEmail;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testParticipantRefId = Guid.NewGuid();
            _testDisplayName = "displayName";
            _testUsername = "username";
            _testUserRole = UserRole.Individual;
            _testHearingRole = "hearingRole";
            _testContactEmail = "contactEmail";
        }
        [Test]
        public void Should_create_participant_without_id()
        {
            var participant = new Participant(_testParticipantRefId, _testDisplayName,
            _testUsername, _testUserRole, _testHearingRole, _testContactEmail);

            ValidateCommonFields(participant);
            participant.Id.Should().NotBeEmpty();            
        }

        [Test]
        public void Should_create_participant_with_id()
        {
            var testId = Guid.NewGuid();

            var participant = new Participant(_testParticipantRefId, _testDisplayName,
            _testUsername, _testUserRole, _testHearingRole, _testContactEmail,
             testId);

            ValidateCommonFields(participant);
            participant.Id.Should().Be(testId);
        }

        private void ValidateCommonFields(Participant participant)
        {
            participant.ParticipantRefId.Should().Be(_testParticipantRefId);
            participant.DisplayName.Should().Be(_testDisplayName);
            participant.Username.Should().Be(_testUsername);
            participant.UserRole.Should().Be(_testUserRole);
            participant.HearingRole.Should().Be(_testHearingRole);
            participant.ContactEmail.Should().Be(_testContactEmail);
        }
    }
}
