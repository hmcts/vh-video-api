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
            var participant = new Participant(TestParticipantRefId, TestName, TestFirstName, TestLastName, TestDisplayName,
            TestUsername, TestUserRole, TestHearingRole, TestCaseTypeGroup, TestContactEmail,
             TestContactTelephone);

            ValidateCommonFields(participant);
            participant.Id.Should().NotBeEmpty();            
        }

        [Test]
        public void Should_create_participant_with_id()
        {
            var testId = Guid.NewGuid();

            var participant = new Participant(TestParticipantRefId, TestName, TestFirstName, TestLastName, TestDisplayName,
            TestUsername, TestUserRole, TestHearingRole, TestCaseTypeGroup, TestContactEmail,
             TestContactTelephone, testId);

            ValidateCommonFields(participant);
            participant.Id.Should().Be(testId);
        }

        private void ValidateCommonFields(Participant participant)
        {
            participant.ParticipantRefId.Should().Be(TestParticipantRefId);
            participant.FirstName.Should().Be(TestFirstName);
            participant.LastName.Should().Be(TestLastName);
            participant.DisplayName.Should().Be(TestDisplayName);
            participant.Username.Should().Be(TestUsername);
            participant.UserRole.Should().Be(TestUserRole);
            participant.HearingRole.Should().Be(TestHearingRole);
            participant.CaseTypeGroup.Should().Be(TestCaseTypeGroup);
            participant.ContactEmail.Should().Be(TestContactEmail);
            participant.ContactTelephone.Should().Be(TestContactTelephone);
        }
    }
}
