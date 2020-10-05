using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Configuration;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;
using VideoApi.Services;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Kinly;
using VideoApi.Services.Mappers;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Services
{
    public class KinlyPlatformServiceTests
    {
        private Mock<IKinlyApiClient> _kinlyApiClientMock;
        private Mock<ILogger<KinlyPlatformService>> _loggerMock;
        private IOptions<ServicesConfiguration> _servicesConfigOptions;

        private Mock<ILogger<IRoomReservationService>> _loggerRoomReservationMock;
        private IRoomReservationService _roomReservationService;
        private Mock<IKinlySelfTestHttpClient> _kinlySelfTestHttpClient;
        private Mock<IPollyRetryService> _pollyRetryService;
        private IMemoryCache _memoryCache;
        private KinlyPlatformService _kinlyPlatformService;
        private Conference _testConference;

        [SetUp]
        public void Setup()
        {
            _kinlyApiClientMock = new Mock<IKinlyApiClient>();
            _loggerMock = new Mock<ILogger<KinlyPlatformService>>();
            
            _loggerRoomReservationMock = new Mock<ILogger<IRoomReservationService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _roomReservationService = new RoomReservationService(_memoryCache, _loggerRoomReservationMock.Object);
            _kinlySelfTestHttpClient = new Mock<IKinlySelfTestHttpClient>();
            _pollyRetryService = new Mock<IPollyRetryService>();
            
            _servicesConfigOptions = Options.Create(new ServicesConfiguration
            {
                CallbackUri = "CallbackUri", KinlyApiUrl = "KinlyApiUrl"
            });

            _kinlyPlatformService = new KinlyPlatformService(
                _kinlyApiClientMock.Object,
                _servicesConfigOptions,
                _loggerMock.Object,
                _roomReservationService,
                _kinlySelfTestHttpClient.Object,
                _pollyRetryService.Object
            );
            
            _testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant", "rep1@dA.com")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithEndpoint("Endpoint With DA", $"{Guid.NewGuid():N}@test.hearings.com", "rep1@dA.com")
                .WithEndpoint("Endpoint Without DA", $"{Guid.NewGuid():N}@test.hearings.com")
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
            _testConference.Participants[1].UpdateParticipantStatus(ParticipantState.InConsultation);
            _testConference.Participants[4].UpdateParticipantStatus(ParticipantState.InConsultation);
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
        public async Task Should_remove_all_participants_and_endpoints_in_room()
        {
            var room = RoomType.ConsultationRoom1;
            _testConference.Participants[1].UpdateParticipantStatus(ParticipantState.InConsultation);
            _testConference.Participants[1].UpdateCurrentRoom(room);
            _testConference.Endpoints[0].UpdateStatus(EndpointState.InConsultation);
            _testConference.Endpoints[0].UpdateCurrentRoom(room);

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
                    _kinlyPlatformService.BookVirtualCourtroomAsync(_testConference.Id, false, "",
                        new List<EndpointDto>()))
                .ErrorMessage.Should().Be($"Meeting room for conference {_testConference.Id} has already been booked");
        }

        [Test]
        public void Should_throw_kinly_api_exception_when_booking_courtroom()
        {
            _kinlyApiClientMock
                .Setup(x => x.CreateHearingAsync(It.IsAny<CreateHearingParams>()))
                .ThrowsAsync(new KinlyApiException("", StatusCodes.Status500InternalServerError, "", null, It.IsAny<Exception>()));

            Assert.ThrowsAsync<KinlyApiException>(() =>
                _kinlyPlatformService.BookVirtualCourtroomAsync(_testConference.Id, false, "", new List<EndpointDto>()));
        }
        
        [Test]
        public async Task Should_return_meeting_room_when_booking_courtroom()
        {
            const bool audioRecordingRequired = false;
            const string ingestUrl = null;
            var endpoints = new List<EndpointDto>
            {
                new EndpointDto{Id = Guid.NewGuid(), Pin = "1234", DisplayName = "one", SipAddress = "99191919"},
                new EndpointDto{Id = Guid.NewGuid(), Pin = "5678", DisplayName = "two", SipAddress = "5385983832"}
            };
            
            var hearingParams = new CreateHearingParams
            {
                Virtual_courtroom_id = _testConference.Id.ToString(),
                Callback_uri = _servicesConfigOptions.Value.CallbackUri,
                Recording_enabled = audioRecordingRequired,
                Recording_url = ingestUrl,
                Streaming_enabled = false,
                Streaming_url = null,
                Jvs_endpoint = endpoints.Select(EndpointMapper.MapToEndpoint).ToList()
            };

            var uris = new Uris
            {
                Admin = "admin", Participant = "participant", Pexip_node = "pexip"
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

            var result = await _kinlyPlatformService.BookVirtualCourtroomAsync(_testConference.Id,
                audioRecordingRequired,
                ingestUrl,
                endpoints);

            result.Should().NotBeNull();
            result.AdminUri.Should().Be(uris.Admin);
            result.JudgeUri.Should().Be(uris.Participant);
            result.ParticipantUri.Should().Be(uris.Participant);
            result.PexipNode.Should().Be(uris.Pexip_node);
            
            _kinlyApiClientMock.Verify(x => x.CreateHearingAsync(It.Is<CreateHearingParams>(param =>
                param.Virtual_courtroom_id == hearingParams.Virtual_courtroom_id &&
                param.Callback_uri == hearingParams.Callback_uri &&
                param.Recording_enabled == hearingParams.Recording_enabled &&
                param.Recording_url == hearingParams.Recording_url &&
                param.Streaming_enabled == hearingParams.Streaming_enabled &&
                param.Streaming_url == hearingParams.Streaming_url &&
                param.Jvs_endpoint != null && param.Jvs_endpoint.Count == hearingParams.Jvs_endpoint.Count
            )), Times.Once);
        }

        [Test]
        public async Task Should_update_virtual_court_room()
        {
            _kinlyApiClientMock.Setup(x => x.UpdateHearingAsync(It.IsAny<string>(), It.IsAny<UpdateHearingParams>()));

            var conferenceId = Guid.NewGuid();
            await _kinlyPlatformService.UpdateVirtualCourtRoomAsync(conferenceId, true, new List<EndpointDto>());
            
            _kinlyApiClientMock.Verify(x => x.UpdateHearingAsync(conferenceId.ToString(), It.Is<UpdateHearingParams>(p => p.Recording_enabled)), Times.Once);
        }

        [Test]
        public async Task Should_get_kinly_virtual_court_room()
        {
            var hearing = new Hearing
            {
                Uris = new Uris
                {
                    Admin = "https://Admin.com", Participant = "https://Participant.com",
                    Pexip_node = "https://Pexip_node.com"
                }
            };

            _kinlyApiClientMock.Setup(x => x.GetHearingAsync(It.IsAny<string>())).ReturnsAsync(hearing);

            var result = await _kinlyPlatformService.GetVirtualCourtRoomAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.AdminUri.Should().Be(hearing.Uris.Admin);
            result.JudgeUri.Should().Be(hearing.Uris.Participant);
            result.ParticipantUri.Should().Be(hearing.Uris.Participant);
        }

        [Test]
        public async Task Should_return_null_for_kinly_virtual_court_room_when_not_found()
        {
            var exception = new KinlyApiException("notfound", StatusCodes.Status404NotFound, "", null, null);
            _kinlyApiClientMock.Setup(x => x.GetHearingAsync(It.IsAny<string>())).Throws(exception);

            var result = await _kinlyPlatformService.GetVirtualCourtRoomAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }

        [Test]
        public void Should_throw_for_kinly_virtual_court_room_when_other_status()
        {
            var exception = new KinlyApiException("BadGateway", StatusCodes.Status502BadGateway, "", null, null);
            _kinlyApiClientMock.Setup(x => x.GetHearingAsync(It.IsAny<string>())).Throws(exception);

            Assert.ThrowsAsync<KinlyApiException>(() => _kinlyPlatformService.GetVirtualCourtRoomAsync(It.IsAny<Guid>()));
        }

        [Test]
        public async Task Should_return_result_from_get_test_call_score()
        {
            var participantId = Guid.NewGuid();
            var expectedTestCallResult = new TestCallResult(true, TestScore.Good);
            
            _pollyRetryService.Setup(x => x.WaitAndRetryAsync<Exception, TestCallResult>
            (
                It.IsAny<int>(), It.IsAny<Func<int, TimeSpan>>(), It.IsAny<Action<int>>(), It.IsAny<Func<TestCallResult, bool>>(), It.IsAny<Func<Task<TestCallResult>>>()
            ))
            .Callback(async (int retries, Func<int, TimeSpan> sleepDuration, Action<int> retryAction, Func<TestCallResult, bool> handleResultCondition, Func<Task<TestCallResult>> executeFunction) =>
            {
                sleepDuration(1);
                retryAction(1);
                handleResultCondition(expectedTestCallResult);
                await executeFunction();
            })
            .ReturnsAsync(expectedTestCallResult);

            var result = await _kinlyPlatformService.GetTestCallScoreAsync(participantId);
            
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTestCallResult);
        }

        [Test]
        public async Task Should_delete_virtual_court_room()
        {
            var conferenceId = Guid.NewGuid();
            _kinlyApiClientMock.Setup(x => x.DeleteHearingAsync(conferenceId.ToString()));

            await _kinlyPlatformService.DeleteVirtualCourtRoomAsync(conferenceId);
            
            _kinlyApiClientMock.Verify(x => x.DeleteHearingAsync(conferenceId.ToString()), Times.Once);
        }

        [Test]
        public async Task should_start_hearing_with_automatic_layout_as_default()
        {
            var conferenceId = Guid.NewGuid();
            await _kinlyPlatformService.StartHearingAsync(conferenceId);
            _kinlyApiClientMock.Verify(
                x => x.StartHearingAsync(conferenceId.ToString(),
                    It.Is<StartHearingParams>(l => l.Layout == Layout.AUTOMATIC)), Times.Once);
        }
        
        [Test]
        public async Task should_start_hearing_with_provided_layout()
        {
            var conferenceId = Guid.NewGuid();
            var layout = Layout.ONE_PLUS_SEVEN;
            await _kinlyPlatformService.StartHearingAsync(conferenceId, layout);
            _kinlyApiClientMock.Verify(
                x => x.StartHearingAsync(conferenceId.ToString(),
                    It.Is<StartHearingParams>(l => l.Layout == layout)), Times.Once);
        }
        
        [Test]
        public async Task should_pause_hearing()
        {
            var conferenceId = Guid.NewGuid();
            await _kinlyPlatformService.PauseHearingAsync(conferenceId);
            _kinlyApiClientMock.Verify(x => x.PauseHearingAsync(conferenceId.ToString()), Times.Once);
        }
        
        [Test]
        public async Task should_end_hearing()
        {
            var conferenceId = Guid.NewGuid();
            await _kinlyPlatformService.EndHearingAsync(conferenceId);
            _kinlyApiClientMock.Verify(x => x.EndHearingAsync(conferenceId.ToString()), Times.Once);
        }

        [Test]
        public async Task should_start_private_consultation_with_endpoint()
        {
            var room = RoomType.ConsultationRoom1;
            var endpointWithDefenceAdvocate = _testConference.GetEndpoints().First(x => !string.IsNullOrWhiteSpace(x.DefenceAdvocate));
            var defenceAdvocate = _testConference.GetParticipants().First(x =>
                x.Username.Equals(endpointWithDefenceAdvocate.DefenceAdvocate,
                    StringComparison.CurrentCultureIgnoreCase));

            endpointWithDefenceAdvocate.UpdateCurrentRoom(RoomType.WaitingRoom);
            defenceAdvocate.UpdateCurrentRoom(RoomType.WaitingRoom);

            
            await _kinlyPlatformService.StartEndpointPrivateConsultationAsync(_testConference, endpointWithDefenceAdvocate,
                defenceAdvocate);

            _kinlyApiClientMock.Verify(x =>
                    x.TransferParticipantAsync(_testConference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.Part_id == endpointWithDefenceAdvocate.Id.ToString() &&
                            r.From == endpointWithDefenceAdvocate.GetCurrentRoom().ToString() &&
                            r.To == room.ToString())
                    )
                , Times.Once);

            _kinlyApiClientMock.Verify(x =>
                    x.TransferParticipantAsync(_testConference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.Part_id == defenceAdvocate.Id.ToString() &&
                            r.From == endpointWithDefenceAdvocate.GetCurrentRoom().ToString() &&
                            r.To == room.ToString())
                    )
                , Times.Once);
        }
    }
}
