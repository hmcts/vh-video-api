using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class AnonymiseQuickLinkParticipantWithHearingIdsRequestValidationTests
    {
        private AnonymiseQuickLinkParticipantWithHearingIdsRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AnonymiseQuickLinkParticipantWithHearingIdsRequestValidation();
        }

        [Test]
        public async Task Fails_Validation_For_Empty_List_Of_Hearingds()
        {
            var request = new AnonymiseQuickLinkParticipantWithHearingIdsRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x =>
                x.ErrorMessage == AnonymiseQuickLinkParticipantWithHearingIdsRequestValidation
                    .NoHearingIdsErrorMessage);
        }

        [Test]
        public async Task Passes_Validation_When_HearingIds_Is_Not_Empty()
        {
            var request = new AnonymiseQuickLinkParticipantWithHearingIdsRequest{HearingIds = new List<Guid>{Guid.NewGuid()}};

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
    }
}
