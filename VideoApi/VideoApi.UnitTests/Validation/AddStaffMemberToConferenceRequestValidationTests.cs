using System.Threading.Tasks;
using VideoApi.Contract.Consts;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Enums;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class AddStaffMemberToConferenceRequestValidationTests
    {
        private AddStaffMemberToConferenceRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddStaffMemberToConferenceRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();
        
            var result = await _validator.ValidateAsync(request);
        
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_participants_error()
        {
            var request = BuildRequest();
            request.FirstName = string.Empty;
            request.ContactEmail = string.Empty;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors.Exists(x => x.PropertyName == "FirstName").Should().BeTrue();
            result.Errors.Exists(x => x.PropertyName == "ContactEmail").Should().BeTrue();
        }
        private static AddStaffMemberRequest BuildRequest()
        {
            return new AddStaffMemberRequest
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Username = "Staff_UserName",
                HearingRole = HearingRoleName.StaffMember,
                Name = "FullName",
                DisplayName = "DisplayName",
                UserRole = UserRole.StaffMember,
                ContactEmail = "FirstName_LastName@hmcts.net"
            };
        }
    }
}
