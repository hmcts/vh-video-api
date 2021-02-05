using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class UpdateParticipantStatusRequestValidationTests
    {
        private UpdateParticipantStatusRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateParticipantStatusRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        private UpdateParticipantStatusRequest BuildRequest()
        {
            return Builder<UpdateParticipantStatusRequest>.CreateNew()
                .With(x => x.State = ParticipantState.InHearing)
                .Build();
        }
    }
}
