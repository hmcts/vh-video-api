using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Contract.Enums;
using VideoApi.Domain;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Clients.Models;
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
            _supplierApiClientMock = new Mock<ISupplierApiClient>();
            _supplierConfig = new VodafoneConfiguration
            {
                CallbackUri = "CallbackUri", ApiUrl = "VodafoneApiUrl"
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
                Supplier.Vodafone
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
                .Setup(x => x.CreateHearingAsync(It.IsAny<BookHearingRequest>()))
                .ThrowsAsync(new SupplierApiException(HttpStatusCode.Conflict, "", It.IsAny<Exception>()));

            Assert.ThrowsAsync<DoubleBookingException>(() =>
                    _supplierPlatformService.BookVirtualCourtroomAsync(_testConference.Id, false, "", new List<EndpointDto>(), It.IsAny<string>(), It.IsAny<ConferenceRoomType>(), It.IsAny<AudioPlaybackLanguage>()))
                ?.ErrorMessage.Should().Be($"Meeting room for conference {_testConference.Id} has already been booked");
        }
        
        [Test]
        public void Should_throw_supplier_api_exception_when_booking_courtroom()
        {
            _supplierApiClientMock
                .Setup(x => x.CreateHearingAsync(It.IsAny<BookHearingRequest>()))
                .ThrowsAsync(new SupplierApiException(HttpStatusCode.InternalServerError, "", It.IsAny<Exception>()));

            Assert.ThrowsAsync<SupplierApiException>(() =>
                _supplierPlatformService.BookVirtualCourtroomAsync(_testConference.Id, false, "", new List<EndpointDto>(), It.IsAny<string>(), It.IsAny<ConferenceRoomType>(), It.IsAny<AudioPlaybackLanguage>()));
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
            
            var hearingParams = new BookHearingRequest
            {
                VirtualCourtroomId = _testConference.Id,
                CallbackUri = _supplierConfig.CallbackUri,
                RecordingEnabled = audioRecordingRequired,
                RecordingUrl = ingestUrl,
                StreamingEnabled = false,
                StreamingUrl = null,
                JvsEndpoint = endpoints.Select(EndpointMapper.MapToEndpoint).ToList(),
                RoomType = conferenceRoomTypeAsString,
                AudioPlaybackLanguage = audioPlaybackLanguage
            };

            var uris = new MeetingUris
            {
                Participant = "participant", PexipNode = "pexip", HearingRoomUri = "meeting_url_alias"
            };
            
            _supplierApiClientMock
                .Setup(x => x.CreateHearingAsync(It.Is<BookHearingRequest>(param =>
                    param.VirtualCourtroomId == hearingParams.VirtualCourtroomId &&
                    param.CallbackUri == hearingParams.CallbackUri &&
                    param.RecordingEnabled == hearingParams.RecordingEnabled &&
                    param.RecordingUrl == hearingParams.RecordingUrl &&
                    param.StreamingEnabled == hearingParams.StreamingEnabled &&
                    param.StreamingUrl == hearingParams.StreamingUrl &&
                    param.RoomType == hearingParams.RoomType
                )))
                .ReturnsAsync(() => new BookHearingResponse
                {
                    Uris = uris,
                    VirtualCourtroomId = Guid.NewGuid()
                });

            var result = await _supplierPlatformService.BookVirtualCourtroomAsync(_testConference.Id,
                audioRecordingRequired,
                ingestUrl,
                endpoints, It.IsAny<string>(), conferenceRoomType, playbackLanguage);

            result.Should().NotBeNull();
            result.AdminUri.Should().Be(uris.Participant);
            result.JudgeUri.Should().Be(uris.Participant);
            result.ParticipantUri.Should().Be(uris.Participant);
            result.PexipNode.Should().Be(uris.PexipNode);
            
            _supplierApiClientMock.Verify(x => x.CreateHearingAsync(It.Is<BookHearingRequest>(param =>
                param.VirtualCourtroomId == hearingParams.VirtualCourtroomId &&
                param.CallbackUri == hearingParams.CallbackUri &&
                param.RecordingEnabled == hearingParams.RecordingEnabled &&
                param.RecordingUrl == hearingParams.RecordingUrl &&
                param.StreamingEnabled == hearingParams.StreamingEnabled &&
                param.StreamingUrl == hearingParams.StreamingUrl &&
                param.RoomType == hearingParams.RoomType &&
                param.JvsEndpoint != null && param.JvsEndpoint.Count == hearingParams.JvsEndpoint.Count &&
                param.JvsEndpoint.TrueForAll(e => e.Role == conferenceRoleAsString)
            )), Times.Once);
        }
        
        [Test]
        public async Task Should_update_virtual_court_room()
        {
            _supplierApiClientMock.Setup(x => x.UpdateHearingAsync(It.IsAny<Guid>(), It.IsAny<UpdateHearingRequest>()));

            var conferenceId = Guid.NewGuid();
            await _supplierPlatformService.UpdateVirtualCourtRoomAsync(conferenceId, true, new List<EndpointDto>(), It.IsAny<ConferenceRoomType>(), It.IsAny<AudioPlaybackLanguage>());
            
            _supplierApiClientMock.Verify(x => x.UpdateHearingAsync(conferenceId, It.Is<UpdateHearingRequest>(p => p.RecordingEnabled)), Times.Once);
        }
        
        [Test]
        public async Task Should_get_supplier_virtual_court_room()
        {
            var hearing = new RetrieveHearingResponse
            {
                Uris = new MeetingUris
                {
                    Participant = "https://Participant.com",
                    PexipNode = "https://Pexip_node.com"
                },
                VirtualCourtroomId = Guid.NewGuid()
            };

            _supplierApiClientMock.Setup(x => x.GetHearingAsync(It.IsAny<Guid>())).ReturnsAsync(hearing);

            var result = await _supplierPlatformService.GetVirtualCourtRoomAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.AdminUri.Should().Be(hearing.Uris.Participant);
            result.JudgeUri.Should().Be(hearing.Uris.Participant);
            result.ParticipantUri.Should().Be(hearing.Uris.Participant);
        }
        
        [Test]
        public async Task Should_return_null_for_supplier_virtual_court_room_when_not_found()
        {
            var exception = new SupplierApiException(HttpStatusCode.NotFound, "", null);
            _supplierApiClientMock.Setup(x => x.GetHearingAsync(It.IsAny<Guid>())).Throws(exception);

            var result = await _supplierPlatformService.GetVirtualCourtRoomAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public void Should_throw_for_supplier_virtual_court_room_when_other_status()
        {
            var exception = new SupplierApiException(HttpStatusCode.BadGateway, "", null);
            _supplierApiClientMock.Setup(x => x.GetHearingAsync(It.IsAny<Guid>())).Throws(exception);

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
            .Callback(async (int _, Func<int, TimeSpan> sleepDuration, Action<int> retryAction, Func<TestCallResult, bool> handleResultCondition, Func<Task<TestCallResult>> executeFunction) =>
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
            _supplierApiClientMock.Setup(x => x.DeleteHearingAsync(conferenceId));

            await _supplierPlatformService.DeleteVirtualCourtRoomAsync(conferenceId);
            
            _supplierApiClientMock.Verify(x => x.DeleteHearingAsync(conferenceId), Times.Once);
        }
        
        [Test]
        public async Task should_start_hearing_with_automatic_layout_as_default()
        {
            var conferenceId = Guid.NewGuid();
            await _supplierPlatformService.StartHearingAsync(conferenceId, It.IsAny<string>());
            _supplierApiClientMock.Verify(
                x => x.StartAsync(conferenceId,
                    It.Is<StartHearingRequest>(l => l.HearingLayout == Layout.Automatic.ToString())), Times.Once);
        }
        
        [Test]
        public async Task should_start_hearing_with_provided_layout()
        {
            var conferenceId = Guid.NewGuid();
            var layout = Layout.OnePlusSeven;
            var participantsToForceTransfer = new[] {"participant-one", "participant-two"};
            var hostIds = new[] {"host-one", "host-two"};
            var muteGuests = false;
            await _supplierPlatformService.StartHearingAsync(conferenceId, It.IsAny<string>(),
                participantsToForceTransfer, hostIds, layout, muteGuests);
            _supplierApiClientMock.Verify(
                x => x.StartAsync(conferenceId,
                    It.Is<StartHearingRequest>(l =>
                        l.HearingLayout == layout.ToString() &&
                        l.ForceTransferParticipantIds.SequenceEqual(participantsToForceTransfer) &&
                        l.Hosts.SequenceEqual(hostIds) && l.MuteGuests == muteGuests)), Times.Once);
        }
        
        [Test]
        public async Task should_pause_hearing()
        {
            var conferenceId = Guid.NewGuid();
            await _supplierPlatformService.PauseHearingAsync(conferenceId);
            _supplierApiClientMock.Verify(x => x.PauseHearingAsync(conferenceId), Times.Once);
        }
        
        [Test]
        public async Task should_end_hearing()
        {
            var conferenceId = Guid.NewGuid();
            await _supplierPlatformService.EndHearingAsync(conferenceId);
            _supplierApiClientMock.Verify(x => x.EndHearingAsync(conferenceId), Times.Once);
        }
        
        [Test]	
        public async Task should_suspend_hearing()	
        {	
            var conferenceId = Guid.NewGuid();	
            await _supplierPlatformService.SuspendHearingAsync(conferenceId);	
            _supplierApiClientMock.Verify(x => x.TechnicalAssistanceAsync(conferenceId), Times.Once);	
        }
        
        [Test]
        public async Task should_update_participant_display_name()
        {
            var conferenceId = Guid.NewGuid();
            var participantId = Guid.NewGuid();
            var name = "New Name";
            await _supplierPlatformService.UpdateParticipantName(conferenceId, participantId, name);
            _supplierApiClientMock.Verify(x => x.UpdateParticipantDisplayNameAsync(conferenceId,
                It.Is<DisplayNameRequest>(p
                    => p.ParticipantId == participantId.ToString() && p.ParticipantName == name)), Times.Once);
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
                x => x.TransferParticipantAsync(conferenceId,
                    It.Is<TransferRequest>(r =>
                        r.Role == expectedRole && r.From == fromRoom && r.To == toRoom && r.PartId == participantId)),
                Times.Once);
        }
    }
}
