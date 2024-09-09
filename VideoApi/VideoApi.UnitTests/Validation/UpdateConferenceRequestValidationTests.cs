using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class UpdateConferenceRequestValidationTests
    {
        private UpdateConferenceRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateConferenceRequestValidation();
        }
        
        private UpdateConferenceRequest BuildRequest()
        {
            return Builder<UpdateConferenceRequest>.CreateNew()
                .With(x => x.ScheduledDateTime = DateTime.Now.AddDays(5))
                .With(x => x.ScheduledDuration = 120)
                .Build();
        }
        
        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();
            
            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_hearing_ref_id_error()
        {
            var request = BuildRequest();
            request.HearingRefId = Guid.Empty;
            
            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewConferenceRequestValidation.NoHearingRefIdErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_invalid_datetime_error()
        {
            var request = BuildRequest();
            request.ScheduledDateTime = DateTime.MinValue;
            
            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x =>
                    x.ErrorMessage == BookNewConferenceRequestValidation.ScheduleDateTimeInPastErrorMessage)
                .Should().BeTrue();
        }
    }
}
