using System;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain
{
    public class ParticipantTests
    {
        private Guid TestParticipantRefId;
        private string TestName;
        private string TestFirstName;
        private string TestLastName;
        private string TestDisplayName;
        private string TestUsername;
        private UserRole TestUserRole;
        private string TestHearingRole;
        private string TestCaseTypeGroup;
        private string TestContactEmail;
        private string TestContactTelephone;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestParticipantRefId = Guid.NewGuid();
            TestName = "name";
            TestFirstName = "firstName";
            TestLastName = "lastName";
            TestDisplayName = "displayName";
            TestUsername = "username";
            TestUserRole = UserRole.Individual;
            TestHearingRole = "hearingRole";
            TestCaseTypeGroup = "caseTypeGroup";
            TestContactEmail = "contactEmail";
            TestContactTelephone = "contactTelephone";
        }
        [Test]
        public void Should_create_participant_without_id()
        {
            var participant = new Participant(TestParticipantRefId, TestDisplayName,
            TestUsername, TestUserRole, TestHearingRole, TestContactEmail);

            ValidateCommonFields(participant);
            participant.Id.Should().NotBeEmpty();            
        }

        [Test]
        public void Should_create_participant_with_id()
        {
            var testId = Guid.NewGuid();

            var participant = new Participant(TestParticipantRefId, TestDisplayName,
            TestUsername, TestUserRole, TestHearingRole, TestContactEmail,
             testId);

            ValidateCommonFields(participant);
            participant.Id.Should().Be(testId);
        }

        private void ValidateCommonFields(Participant participant)
        {
            participant.ParticipantRefId.Should().Be(TestParticipantRefId);
            participant.DisplayName.Should().Be(TestDisplayName);
            participant.Username.Should().Be(TestUsername);
            participant.UserRole.Should().Be(TestUserRole);
            participant.HearingRole.Should().Be(TestHearingRole);
            participant.ContactEmail.Should().Be(TestContactEmail);
        }
    }
}
