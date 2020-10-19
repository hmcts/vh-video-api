using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;

namespace VideoApi.UnitTests.Validation
{
    public class StartHearingRequestValidationTests
    {
        private StartHearingRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new StartHearingRequestValidation();
        }

        [Test]
        public async Task should_pass_validation()
        {
            var request = new StartHearingRequest
            {
                Layout = HearingLayout.Dynamic
            };
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
    }
}
