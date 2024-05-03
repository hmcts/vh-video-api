using System;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class GetConferencesByHearingIdsRequestValidationTests
    {
        private GetConferencesByHearingIdsRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _validator = new GetConferencesByHearingIdsRequestValidation();
        }

        [Test]
        public async Task fails_when_hearing_ids_are_not_specified()
        {
            var request = new GetConferencesByHearingIdsRequest
            {
                HearingRefIds = null
            };

            var result = await _validator.ValidateAsync(request);

            result.Errors.Any(x => x.ErrorMessage == GetConferencesByHearingIdsRequestValidation.HearingIdNotSpecifiedError).Should().BeTrue();
        }

        [Test]
        public async Task passes_validation()
        {
            var request = new GetConferencesByHearingIdsRequest
            {
                HearingRefIds = new []{Guid.NewGuid()}
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
    }
}
