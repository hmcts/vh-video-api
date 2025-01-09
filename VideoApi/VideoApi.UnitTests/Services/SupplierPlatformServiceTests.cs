using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Contract.Enums;
using VideoApi.Domain;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Mappers;
using Task = System.Threading.Tasks.Task;
using TestScore = VideoApi.Domain.Enums.TestScore;
using UserRole = VideoApi.Domain.Enums.UserRole;
using Supplier = VideoApi.Domain.Enums.Supplier;
using ConferenceRoomType = VideoApi.Domain.Enums.ConferenceRoomType;
using AudioPlaybackLanguage = VideoApi.Domain.Enums.AudioPlaybackLanguage;

namespace VideoApi.UnitTests.Services
{
    public class SupplierPlatformServiceTests
    {
        private Mock<IFeatureToggles> _featureToggles;
        private Mock<ILogger<SupplierPlatformService>> _loggerMock;
        private Mock<IPollyRetryService> _pollyRetryService;
        private Mock<ISupplierApiClient> _supplierApiClientMock;
        private SupplierConfiguration _supplierConfig;
        private SupplierPlatformService _supplierPlatformService;
        private Mock<ISupplierSelfTestHttpClient> _supplierSelfTestHttpClient;
        private Conference _testConference;
        
        [SetUp]
        public void Setup()
        {
            _featureToggles = new Mock<IFeatureToggles>();
            _featureToggles.Setup(x => x.SendTransferRolesEnabled()).Returns(true);
            _supplierApiClientMock = new Mock<ISupplierApiClient>();
            _supplierConfig = new KinlyConfiguration()
            {
                CallbackUri = "CallbackUri", ApiUrl = "KinlyApiUrl"
            };
            _loggerMock = new Mock<ILogger<SupplierPlatformService>>();

            _supplierSelfTestHttpClient = new Mock<ISupplierSelfTestHttpClient>();
            _pollyRetryService = new Mock<IPollyRetryService>();
            
            _supplierPlatformService = new SupplierPlatformService(
                _loggerMock.Object,
                _supplierSelfTestHttpClient.Object,
                _pollyRetryService.Object,
                _supplierApiClientMock.Object,
                _supplierConfig,
                Supplier.Vodafone,
                _featureToggles.Object
            );
            
            _testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithParticipant(UserRole.Representative, "Applicant", "rep1@hmcts.net")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithEndpoint("Endpoint With DA", $"{Guid.NewGuid():N}@hmcts.net", "rep1@hmcts.net")
                .WithEndpoint("Endpoint Without DA", $"{Guid.NewGuid():N}@hmcts.net")
                .Build();
        }
        
        [Test]
        public void Should_throw_double_booking_exception_on_conflict_return_status_code_when_booking_courtroom()
        {
            _supplierApiClientMock
                .Setup(x => x.CreateHearingAsync(It.IsAny<CreateHearingParams>()))
                .ThrowsAsync(new SupplierApiException("", StatusCodes.Status409Conflict, "", null, It.IsAny<Exception>()));

            Assert.ThrowsAsync<DoubleBookingException>(() =>
                    _supplierPlatformService.BookVirtualCourtroomAsync(_testConference.Id, false, "", new List<EndpointDto>(), It.IsAny<string>(), It.IsAny<VideoApi.Domain.Enums.ConferenceRoomType>(), It.IsAny<VideoApi.Domain.Enums.AudioPlaybackLanguage>()))
                .ErrorMessage.Should().Be($"Meeting room for conference {_testConference.Id} has already been booked");
        }
        
        [Test]
        public void Should_throw_supplier_api_exception_when_booking_courtroom()
        {
            _supplierApiClientMock
                .Setup(x => x.CreateHearingAsync(It.IsAny<CreateHearingParams>()))
                .ThrowsAsync(new SupplierApiException("", StatusCodes.Status500InternalServerError, "", null, It.IsAny<Exception>()));

            Assert.ThrowsAsync<SupplierApiException>(() =>
                _supplierPlatformService.BookVirtualCourtroomAsync(_testConference.Id, false, "", new List<EndpointDto>(), It.IsAny<string>(), It.IsAny<VideoApi.Domain.Enums.ConferenceRoomType>(), It.IsAny<VideoApi.Domain.Enums.AudioPlaybackLanguage>()));
        }
        
        [Test]
        public async Task Should_return_meeting_room_when_booking_courtroom()
        {
            const bool audioRecordingRequired = false;
            const string ingestUrl = null;
            const string conferenceRoleAsString = "Guest";
            var conferenceRole = (ConferenceRole)Enum.Parse(typeof(ConferenceRole), conferenceRoleAsString);
            const string conferenceRoomTypeAsString = "VA";
            const string audioPlaybackLanguage = "English";
            var conferenceRoomType = (ConferenceRoomType)Enum.Parse(typeof(ConferenceRoomType), conferenceRoomTypeAsString);
            var playbackLanguage = (AudioPlaybackLanguage)Enum.Parse(typeof(AudioPlaybackLanguage), audioPlaybackLanguage);
            var endpoints = new List<EndpointDto>
            {
                new () {Id = Guid.NewGuid(), Pin = "1234", DisplayName = "one", SipAddress = "99191919", ConferenceRole = conferenceRole },
                new () {Id = Guid.NewGuid(), Pin = "5678", DisplayName = "two", SipAddress = "5385983832", ConferenceRole = conferenceRole }
            };
            
            var hearingParams = new CreateHearingParams
            {
                Virtual_courtroom_id = _testConference.Id.ToString(),
                Callback_uri = _supplierConfig.CallbackUri,
                Recording_enabled = audioRecordingRequired,
                Recording_url = ingestUrl,
                Streaming_enabled = false,
                Streaming_url = null,
                Jvs_endpoint = endpoints.Select(EndpointMapper.MapToEndpoint).ToList(),
                RoomType = conferenceRoomTypeAsString,
                AudioPlaybackLanguage = audioPlaybackLanguage
            };

            var uris = new Uris
            {
                Admin = "admin", Participant = "participant", Pexip_node = "pexip"
            };
            
            _supplierApiClientMock
                .Setup(x => x.CreateHearingAsync(It.Is<CreateHearingParams>(param =>
                    param.Virtual_courtroom_id == hearingParams.Virtual_courtroom_id &&
                    param.Callback_uri == hearingParams.Callback_uri &&
                    param.Recording_enabled == hearingParams.Recording_enabled &&
                    param.Recording_url == hearingParams.Recording_url &&
                    param.Streaming_enabled == hearingParams.Streaming_enabled &&
                    param.Streaming_url == hearingParams.Streaming_url &&
                    param.RoomType == hearingParams.RoomType
                )))
                .ReturnsAsync(() => new Hearing
                {
                    Uris = uris,
                    Telephone_conference_id = "12345678",
                    Virtual_courtroom_id = Guid.NewGuid()
                });

            var result = await _supplierPlatformService.BookVirtualCourtroomAsync(_testConference.Id,
                audioRecordingRequired,
                ingestUrl,
                endpoints, It.IsAny<string>(), conferenceRoomType, playbackLanguage);

            result.Should().NotBeNull();
            result.AdminUri.Should().Be(uris.Admin);
            result.JudgeUri.Should().Be(uris.Participant);
            result.ParticipantUri.Should().Be(uris.Participant);
            result.PexipNode.Should().Be(uris.Pexip_node);
            
            _supplierApiClientMock.Verify(x => x.CreateHearingAsync(It.Is<CreateHearingParams>(param =>
                param.Virtual_courtroom_id == hearingParams.Virtual_courtroom_id &&
                param.Callback_uri == hearingParams.Callback_uri &&
                param.Recording_enabled == hearingParams.Recording_enabled &&
                param.Recording_url == hearingParams.Recording_url &&
                param.Streaming_enabled == hearingParams.Streaming_enabled &&
                param.Streaming_url == hearingParams.Streaming_url &&
                param.RoomType == hearingParams.RoomType &&
                param.Jvs_endpoint != null && param.Jvs_endpoint.Count == hearingParams.Jvs_endpoint.Count &&
                param.Jvs_endpoint.TrueForAll(e => e.Role == conferenceRoleAsString)
            )), Times.Once);
        }
        
        [Test]
        public async Task Should_update_virtual_court_room()
        {
            _supplierApiClientMock.Setup(x => x.UpdateHearingAsync(It.IsAny<string>(), It.IsAny<UpdateHearingParams>()));

            var conferenceId = Guid.NewGuid();
            await _supplierPlatformService.UpdateVirtualCourtRoomAsync(conferenceId, true, new List<EndpointDto>(), It.IsAny<VideoApi.Domain.Enums.ConferenceRoomType>(), It.IsAny<VideoApi.Domain.Enums.AudioPlaybackLanguage>());
            
            _supplierApiClientMock.Verify(x => x.UpdateHearingAsync(conferenceId.ToString(), It.Is<UpdateHearingParams>(p => p.Recording_enabled)), Times.Once);
        }
        
        [Test]
        public async Task Should_get_supplier_virtual_court_room()
        {
            var hearing = new Hearing
            {
                Uris = new Uris
                {
                    Admin = "https://Admin.com", Participant = "https://Participant.com",
                    Pexip_node = "https://Pexip_node.com"
                },
                Telephone_conference_id = "12345678",
                Virtual_courtroom_id = Guid.NewGuid()
            };

            _supplierApiClientMock.Setup(x => x.GetHearingAsync(It.IsAny<string>())).ReturnsAsync(hearing);

            var result = await _supplierPlatformService.GetVirtualCourtRoomAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.AdminUri.Should().Be(hearing.Uris.Admin);
            result.JudgeUri.Should().Be(hearing.Uris.Participant);
            result.ParticipantUri.Should().Be(hearing.Uris.Participant);
        }
        
        [Test]
        public async Task Should_return_null_for_supplier_virtual_court_room_when_not_found()
        {
            var exception = new SupplierApiException("notfound", StatusCodes.Status404NotFound, "", null, null);
            _supplierApiClientMock.Setup(x => x.GetHearingAsync(It.IsAny<string>())).Throws(exception);

            var result = await _supplierPlatformService.GetVirtualCourtRoomAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public void Should_throw_for_supplier_virtual_court_room_when_other_status()
        {
            var exception = new SupplierApiException("BadGateway", StatusCodes.Status502BadGateway, "", null, null);
            _supplierApiClientMock.Setup(x => x.GetHearingAsync(It.IsAny<string>())).Throws(exception);

            Assert.ThrowsAsync<SupplierApiException>(() => _supplierPlatformService.GetVirtualCourtRoomAsync(It.IsAny<Guid>()));
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

            var result = await _supplierPlatformService.GetTestCallScoreAsync(participantId);
            
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTestCallResult);
        }
        
        [Test]
        public async Task Should_delete_virtual_court_room()
        {
            var conferenceId = Guid.NewGuid();
            _supplierApiClientMock.Setup(x => x.DeleteHearingAsync(conferenceId.ToString()));

            await _supplierPlatformService.DeleteVirtualCourtRoomAsync(conferenceId);
            
            _supplierApiClientMock.Verify(x => x.DeleteHearingAsync(conferenceId.ToString()), Times.Once);
        }
        
        [Test]
        public async Task should_start_hearing_with_automatic_layout_as_default()
        {
            var conferenceId = Guid.NewGuid();
            await _supplierPlatformService.StartHearingAsync(conferenceId, It.IsAny<string>());
            _supplierApiClientMock.Verify(
                x => x.StartAsync(conferenceId.ToString(),
                    It.Is<StartHearingRequest>(l => l.Hearing_layout == Layout.AUTOMATIC)), Times.Once);
        }
        
        [Test]
        public async Task should_start_hearing_with_provided_layout()
        {
            var conferenceId = Guid.NewGuid();
            var layout = Layout.ONE_PLUS_SEVEN;
            var participantsToForceTransfer = new[] {"participant-one", "participant-two"};
            var hostIds = new[] {"host-one", "host-two"};
            var muteGuests = false;
            await _supplierPlatformService.StartHearingAsync(conferenceId, It.IsAny<string>(),
                participantsToForceTransfer, hostIds, layout, muteGuests);
            _supplierApiClientMock.Verify(
                x => x.StartAsync(conferenceId.ToString(),
                    It.Is<StartHearingRequest>(l =>
                        l.Hearing_layout == layout &&
                        l.Force_transfer_participant_ids.SequenceEqual(participantsToForceTransfer) &&
                        l.Hosts.SequenceEqual(hostIds) && l.Mute_guests == muteGuests)), Times.Once);
        }
        
        [Test]
        public async Task should_pause_hearing()
        {
            var conferenceId = Guid.NewGuid();
            await _supplierPlatformService.PauseHearingAsync(conferenceId);
            _supplierApiClientMock.Verify(x => x.PauseHearingAsync(conferenceId.ToString()), Times.Once);
        }
        
        [Test]
        public async Task should_end_hearing()
        {
            var conferenceId = Guid.NewGuid();
            await _supplierPlatformService.EndHearingAsync(conferenceId);
            _supplierApiClientMock.Verify(x => x.EndHearingAsync(conferenceId.ToString()), Times.Once);
        }
        
        [Test]	
        public async Task should_suspend_hearing()	
        {	
            var conferenceId = Guid.NewGuid();	
            await _supplierPlatformService.SuspendHearingAsync(conferenceId);	
            _supplierApiClientMock.Verify(x => x.TechnicalAssistanceAsync(conferenceId.ToString()), Times.Once);	
        }
        
        [Test]
        public async Task should_update_participant_display_name()
        {
            var conferenceId = Guid.NewGuid();
            var participantId = Guid.NewGuid();
            var name = "New Name";
            await _supplierPlatformService.UpdateParticipantName(conferenceId, participantId, name);
            _supplierApiClientMock.Verify(x => x.UpdateParticipanNameAsync(conferenceId.ToString(),
                It.Is<UpdateParticipantNameParams>(p
                    => p.Participant_Id == participantId.ToString() && p.Participant_Name == name)), Times.Once);
        }
        
        [TestCase(null, null)]
        [TestCase(VideoApi.Domain.Enums.ConferenceRole.Guest, "Guest")]
        [TestCase(VideoApi.Domain.Enums.ConferenceRole.Host, "Host")]
        public async Task should_transfer_with_with_role(VideoApi.Domain.Enums.ConferenceRole? role, string expectedRole)
        {
            // arrange
            var conferenceId = Guid.NewGuid();
            var participantId = Guid.NewGuid().ToString();
            var fromRoom = "WaitingRoom";
            var toRoom = "HearingRoom";
            
            // act
            await _supplierPlatformService.TransferParticipantAsync(conferenceId, participantId, fromRoom, toRoom, role);
            
            // assert
            _supplierApiClientMock.Verify(
                x => x.TransferParticipantAsync(conferenceId.ToString(),
                    It.Is<TransferParticipantParams>(r =>
                        r.Role == expectedRole && r.From == fromRoom && r.To == toRoom && r.Part_id == participantId)),
                Times.Once);
        }
        
        [Test]
        public async Task should_transfer_when_send_role_flag_off()
        {
            // arrange
            _featureToggles.Setup(x => x.SendTransferRolesEnabled()).Returns(false);
            var conferenceId = Guid.NewGuid();
            var participantId = Guid.NewGuid().ToString();
            var fromRoom = "WaitingRoom";
            var toRoom = "HearingRoom";
            
            // act
            await _supplierPlatformService.TransferParticipantAsync(conferenceId, participantId, fromRoom, toRoom, VideoApi.Domain.Enums.ConferenceRole.Guest);
            
            // assert
            _supplierApiClientMock.Verify(
                x => x.TransferParticipantAsync(conferenceId.ToString(),
                    It.Is<TransferParticipantParams>(r =>
                        r.Role == null && r.From == fromRoom && r.To == toRoom && r.Part_id == participantId)),
                Times.Once);
        }
    }
}
