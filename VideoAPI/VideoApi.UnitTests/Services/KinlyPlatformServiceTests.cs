using System;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
using VideoApi.Services.Exceptions;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Services
{
    public class KinlyPlatformServiceTests
    {
        private Mock<IKinlyApiClient> _kinlyApiClientMock;
        private Mock<ICustomJwtTokenProvider> _customJwtTokenProviderMock;
        private Mock<ILogger<KinlyPlatformService>> _loggerMock;
        private IOptions<ServicesConfiguration> _servicesConfigOptions;

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
            
            _loggerRoomReservationMock = new Mock<ILogger<IRoomReservationService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _roomReservationService = new RoomReservationService(_memoryCache, _loggerRoomReservationMock.Object);
            
            _servicesConfigOptions = Options.Create(new ServicesConfiguration
            {
                CallbackUri = "CallbackUri", KinlyApiUrl = "KinlyApiUrl"
            });

            _kinlyPlatformService = new KinlyPlatformService(
                _kinlyApiClientMock.Object,
                _servicesConfigOptions,
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

        [Test]
        public void Should_throw_double_booking_exception_on_conflict_return_status_code_when_booking_courtroom()
        {
            _kinlyApiClientMock
                .Setup(x => x.CreateHearingAsync(It.IsAny<CreateHearingParams>()))
                .ThrowsAsync(new KinlyApiException("", StatusCodes.Status409Conflict, "", null, It.IsAny<Exception>()));

            Assert.ThrowsAsync<DoubleBookingException>(() =>
                _kinlyPlatformService.BookVirtualCourtroomAsync(_testConference.Id, false, ""));
        }
        
        [Test]
        public void Should_throw_kinly_api_exception_when_booking_courtroom()
        {
            _kinlyApiClientMock
                .Setup(x => x.CreateHearingAsync(It.IsAny<CreateHearingParams>()))
                .ThrowsAsync(new KinlyApiException("", StatusCodes.Status500InternalServerError, "", null, It.IsAny<Exception>()));

            Assert.ThrowsAsync<KinlyApiException>(() =>
                _kinlyPlatformService.BookVirtualCourtroomAsync(_testConference.Id, false, ""));
        }
        
        [Test]
        public async Task Should_return_meeting_room_when_booking_courtroom()
        {
            const bool audioRecordingRequired = false;
            const string ingestUrl = null;
            var hearingParams = new CreateHearingParams
            {
                Virtual_courtroom_id = _testConference.Id.ToString(),
                Callback_uri = _servicesConfigOptions.Value.CallbackUri,
                Recording_enabled = audioRecordingRequired,
                Recording_url = ingestUrl,
                Streaming_enabled = false,
                Streaming_url = null
            };

            var uris = new Uris
            {
                Admin = "admin", Judge = "judge", Participant = "participant", Pexip_node = "pexip"
            };
            
            _kinlyApiClientMock
                .Setup(x => x.CreateHearingAsync(It.Is<CreateHearingParams>(param =>
                    param.Virtual_courtroom_id == hearingParams.Virtual_courtroom_id &&
                    param.Callback_uri == hearingParams.Callback_uri &&
                    param.Recording_enabled == hearingParams.Recording_enabled &&
                    param.Recording_url == hearingParams.Recording_url &&
                    param.Streaming_enabled == hearingParams.Streaming_enabled &&
                    param.Streaming_url == hearingParams.Streaming_url
                )))
                .ReturnsAsync(() => new Hearing
                {
                    Uris = uris
                });

            var result = await _kinlyPlatformService.BookVirtualCourtroomAsync(_testConference.Id, audioRecordingRequired, ingestUrl);

            result.Should().NotBeNull();
            result.AdminUri.Should().Be(uris.Admin);
            result.JudgeUri.Should().Be(uris.Judge);
            result.ParticipantUri.Should().Be(uris.Participant);
            result.PexipNode.Should().Be(uris.Pexip_node);
        }

        [Test]
        public async Task Should_update_virtual_court_room()
        {
            _kinlyApiClientMock.Setup(x => x.UpdateHearingAsync(It.IsAny<string>(), It.IsAny<UpdateHearingParams>()));

            await _kinlyPlatformService.UpdateVirtualCourtRoomAsync(Guid.NewGuid(), true);
        }
    }
}
