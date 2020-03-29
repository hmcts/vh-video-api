using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;
using VideoApi.Services;
using FluentAssertions;
using System;
using System.Linq;

namespace VideoApi.UnitTests.Services
{
  
    public class RoomReservationServiceTests
    {
        private Mock<ILogger<IRoomReservationService>> _loggerRoomReservationMock;
        private IMemoryCache _memoryCache;
        private Conference _testConference;
        private IRoomReservationService _roomReservationService;

        [SetUp]
        public void SetUp()
        {
            _loggerRoomReservationMock = new Mock<ILogger<IRoomReservationService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _roomReservationService = new RoomReservationService(_memoryCache, _loggerRoomReservationMock.Object);
            _testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .Build();
        }

        [Test]
        public void Should_throw_exception_when_no_hearing_room_available()
        {
            // make sure no rooms are available
            _testConference.Participants[1].UpdateCurrentRoom(RoomType.ConsultationRoom1);
            _testConference.Participants[2].UpdateCurrentRoom(RoomType.ConsultationRoom2);

            Action action = () => _roomReservationService.GetNextAvailableConsultationRoom(_testConference);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures.Any(x => x.Name == "Unavailable room")
                .Should().BeTrue();

        }

        [Test]
        public void Should_return_first_consultation_room_when_none_are_occupied()
        {
            var availableRoom = _roomReservationService.GetNextAvailableConsultationRoom(_testConference);
            availableRoom.Should().Be(RoomType.ConsultationRoom1);
        }

        [Test]
        public void Should_return_second_consultation_room_when_room1_is_occupied()
        {
            _testConference.Participants[1].UpdateCurrentRoom(RoomType.ConsultationRoom1);
            var availableRoom = _roomReservationService.GetNextAvailableConsultationRoom(_testConference);
            availableRoom.Should().Be(RoomType.ConsultationRoom2);
        }

        [Test]
        public void Should_return_consultation_rooms_in_sequence_when_multiple_requests_are_made()
        {
            _roomReservationService.GetNextAvailableConsultationRoom(_testConference)
                .Should().Be(RoomType.ConsultationRoom1);

            _roomReservationService.GetNextAvailableConsultationRoom(_testConference)
                .Should().Be(RoomType.ConsultationRoom2);
        }

        [Test]
        public void Should_throw_exception_when_multiple_requests_are_made_and_rooms_run_out()
        {
            _roomReservationService.GetNextAvailableConsultationRoom(_testConference)
                .Should().Be(RoomType.ConsultationRoom1);

            _roomReservationService.GetNextAvailableConsultationRoom(_testConference)
                .Should().Be(RoomType.ConsultationRoom2);

            Action action = () => _roomReservationService.GetNextAvailableConsultationRoom(_testConference);
            action.Should().Throw<DomainRuleException>().
                            And.ValidationFailures.Any(x => x.Name == "Unavailable room")
                .Should().BeTrue();
        }
    }
}
