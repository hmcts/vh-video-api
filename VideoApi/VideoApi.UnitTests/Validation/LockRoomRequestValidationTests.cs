using System;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.Validations;
using VideoApi.Contract.Requests;

namespace VideoApi.UnitTests.Validation
{
    public class LockRoomRequestValidationTests
    {
        private LockRoomRequestValidation _sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = new LockRoomRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = new LockRoomRequest
            {
                ConferenceId = Guid.NewGuid(),
                RoomLabel = "RoomLabel",
                Lock = true
            };

            var result = await _sut.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_when_conference_id_empty()
        {
            var request = new LockRoomRequest
            {
                ConferenceId = Guid.Empty,
                RoomLabel = "RoomLabel",
                Lock = true
            };

            var result = await _sut.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == LockRoomRequestValidation.NoConferenceError)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_endpoint_id_is_empty()
        {
            var request = new LockRoomRequest
            {
                ConferenceId = Guid.NewGuid(),
                RoomLabel = string.Empty,
                Lock = true
            };

            var result = await _sut.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == LockRoomRequestValidation.NoRoomLabelError)
                .Should().BeTrue();
        }
    }
}
