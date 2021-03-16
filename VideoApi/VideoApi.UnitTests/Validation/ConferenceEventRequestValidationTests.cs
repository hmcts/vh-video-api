using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Enums;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class ConferenceEventRequestValidationTests
    {
        private ConferenceEventRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new ConferenceEventRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_pass_validation_when_joining_call_to_waiting_room()
        {
            var request = BuildRequest();
            request.TransferFrom = null;
            request.TransferTo = RoomType.WaitingRoom.ToString();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_conference_id_error()
        {
            var request = BuildRequest();
            request.ConferenceId = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == ConferenceEventRequestValidation.NoConferenceIdErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_invalid_conference_id_format_error()
        {
            var request = BuildRequest();
            request.ConferenceId = "uygfuyguds";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x =>
                    x.ErrorMessage == ConferenceEventRequestValidation.InvalidConferenceIdFormatErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_event_id_error()
        {
            var request = BuildRequest();
            request.EventId = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ConferenceEventRequestValidation.NoEventIdErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_participant_id_error()
        {
            var request = BuildRequest();
            request.ParticipantId = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == ConferenceEventRequestValidation.NoParticipantIdErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_invalid_participant_id_error()
        {
            var request = BuildRequest();
            request.ParticipantId = "iughfuidshif";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x =>
                    x.ErrorMessage == ConferenceEventRequestValidation.InvalidParticipantIdFormatErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_event_type_error()
        {
            var request = BuildRequest();
            request.EventType = EventType.None;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ConferenceEventRequestValidation.NoEventTypeErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_valid_for_suspend_event_without_participantid()
        {
            var request = BuildRequest();
            request.EventType = EventType.Suspend;
            request.ParticipantId = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_invalid_for_transfer_event_without_participantid()
        {
            var request = BuildRequest();
            request.EventType = EventType.Transfer;
            request.ParticipantId = string.Empty;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
        }

        [TestCase(EventType.RoomParticipantDisconnected)]
        [TestCase(EventType.RoomParticipantJoined)]
        [TestCase(EventType.RoomParticipantTransfer)]
        public async Task should_return_valid_when_room_participant_event(EventType eventType)
        {
            var request = BuildRoomParticipantRequest();
            request.EventType = eventType;
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(EventType.RoomParticipantDisconnected)]
        [TestCase(EventType.RoomParticipantJoined)]
        [TestCase(EventType.RoomParticipantTransfer)]
        public async Task should_fail_validation_when_room_event_but_room_id_is_missing(EventType eventType)
        {
            var request = BuildRoomParticipantRequest();
            request.EventType = eventType;
            request.ParticipantRoomId = null;
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ConferenceEventRequestValidation.NoParticipantRoomIdErrorMessage)
                .Should().BeTrue();
        }

        private ConferenceEventRequest BuildRequest()
        {
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = Guid.NewGuid().ToString())
                .With(x => x.ParticipantId = Guid.NewGuid().ToString())
                .With(x => x.EventId = Guid.NewGuid().ToString())
                .With(x => x.EventType = EventType.Transfer)
                .With(x => x.TransferFrom = RoomType.WaitingRoom.ToString())
                .With(x => x.TransferTo = "ConsultationRoom")
                .With(x => x.Phone = null)
                .With(x => x.ParticipantRoomId = null)
                .Build();
            return request;
        }

        private ConferenceEventRequest BuildRoomParticipantRequest()
        {
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = Guid.NewGuid().ToString())
                .With(x => x.ParticipantId = Guid.NewGuid().ToString())
                .With(x => x.ParticipantRoomId = "1")
                .With(x => x.EventId = Guid.NewGuid().ToString())
                .With(x => x.EventType = EventType.RoomParticipantJoined)
                .With(x => x.TransferFrom = RoomType.WaitingRoom.ToString())
                .With(x => x.TransferTo = "ConsultationRoom")
                .With(x => x.Phone = null)
                .Build();
            return request;
        }
    }
}
