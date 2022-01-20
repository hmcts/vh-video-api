using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class ConferenceForHostWithSelectedVenueRequestValidationTests
    {
        private ConferenceForHostWithSelectedVenueRequestValidation _validator;


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _validator = new ConferenceForHostWithSelectedVenueRequestValidation();
        }

        [Test]
        public async Task fails_when_hearing_venue_is_not_specified()
        {
            var request = new ConferenceForStaffMembertWithSelectedVenueRequest
            {
                HearingVenueNames = new List<string>()
            };

            var result = await _validator.ValidateAsync(request);

            result.Errors.Any(x => x.ErrorMessage == ConferenceForHostWithSelectedVenueRequestValidation.HearingVenueNotSpecifiedError).Should().BeTrue();
        }

        [Test]
        public async Task passes_validation()
        {
            var request = new ConferenceForStaffMembertWithSelectedVenueRequest
            {
                HearingVenueNames = new List<string>() { "asdsad" }
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
    }
}
