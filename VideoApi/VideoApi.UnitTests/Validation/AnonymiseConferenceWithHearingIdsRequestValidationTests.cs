using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class AnonymiseConferenceWithHearingIdsRequestValidationTests
    {
        private AnonymiseConferenceWithHearingIdsRequestValidation _validator;
        
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AnonymiseConferenceWithHearingIdsRequestValidation();
        }
        
        
        [Test]
        public async Task Fails_Validation_For_Empty_List_Of_HearingIds()
        {
            var request = new AnonymiseConferenceWithHearingIdsRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            var containsErrorMessage = result.Errors.Exists(x =>
                x.ErrorMessage == AnonymiseConferenceWithHearingIdsRequestValidation
                    .NoHearingIdsErrorMessage);
            containsErrorMessage.Should().BeTrue();
        }

        [Test]
        public async Task Passes_Validation_When_HearingIds_Is_Not_Empty()
        {
            var request = new AnonymiseConferenceWithHearingIdsRequest{HearingIds = new List<Guid>{Guid.NewGuid()}};

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
    }
}
