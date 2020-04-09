using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.Kinly;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;
using VideoApi.Services;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Services
{
    public class KinlyPlatformServiceTests
    {
        private Mock<IKinlyApiClient> _kinlyApiClientMock;
        private Mock<ICustomJwtTokenProvider> _customJwtTokenProviderMock;
        private Mock<ILogger<KinlyPlatformService>> _loggerMock;
        private Mock<IOptions<ServicesConfiguration>> _servicesConfigOptionsMock;

        private Mock<ILogger<IRoomReservationService>> _loggerRoomReservationMock;
        private IRoomReservationService _roomReservationService;
        private IMemoryCache _memoryCache;
        private KinlyPlatformService _kinlyPlatformService;
        private Conference _testConference;

        [SetUp]
        public void Setup()
        {
            _kinlyApiClientMock = new Mock<IKinlyApiClient>();
            _customJwtTokenProviderMock = new Mock<ICustomJwtTokenProvider>();
            _loggerMock = new Mock<ILogger<KinlyPlatformService>>();
            _servicesConfigOptionsMock = new Mock<IOptions<ServicesConfiguration>>();

            _loggerRoomReservationMock = new Mock<ILogger<IRoomReservationService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _roomReservationService = new RoomReservationService(_memoryCache, _loggerRoomReservationMock.Object);
            

            _kinlyPlatformService = new KinlyPlatformService(
                _kinlyApiClientMock.Object,
                _servicesConfigOptionsMock.Object,
                _customJwtTokenProviderMock.Object,
                _loggerMock.Object,
                _roomReservationService
            );
            
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
            var requestedBy = _testConference.GetParticipants()[2];
            var requestedFor = _testConference.GetParticipants()[3];
            
            // make sure no rooms are available
            _testConference.Participants[1].UpdateCurrentRoom(RoomType.ConsultationRoom1);
            _testConference.Participants[4].UpdateCurrentRoom(RoomType.ConsultationRoom2);

            Func<Task> sutMethod = async () =>
            {
                await _kinlyPlatformService.StartPrivateConsultationAsync(_testConference, requestedBy,
                    requestedFor);
            };
            sutMethod.Should().Throw<DomainRuleException>().And.ValidationFailures.Any(x => x.Name == "Unavailable room")
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_transfer_participants_when_pc_started()
        {
            var requestedBy = _testConference.GetParticipants()[2];
            var requestedFor = _testConference.GetParticipants()[3];

            await _kinlyPlatformService.StartPrivateConsultationAsync(_testConference, requestedBy,
                requestedFor);

            _kinlyApiClientMock.Verify(x =>
                    x.TransferParticipantAsync(_testConference.Id.ToString(), It.IsAny<TransferParticipantParams>())
                , Times.Exactly(2));
        }

        [Test]
        public async Task Should_remove_all_participants_in_room()
        {
            var room = RoomType.ConsultationRoom1;
            _testConference.Participants[1].UpdateCurrentRoom(room);
            _testConference.Participants[4].UpdateCurrentRoom(room);
            
            await _kinlyPlatformService.StopPrivateConsultationAsync(_testConference, room);
            
            _kinlyApiClientMock.Verify(x =>
                    x.TransferParticipantAsync(_testConference.Id.ToString(), 
                        It.Is<TransferParticipantParams>(r => 
                            r.From == room.ToString() && 
                            r.To == RoomType.WaitingRoom.ToString()
                            )
                        )
                , Times.Exactly(2));
        } 
    }
}
