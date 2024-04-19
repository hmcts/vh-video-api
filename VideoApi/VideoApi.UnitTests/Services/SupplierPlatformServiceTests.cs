using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Clients;
using VideoApi.Services.Mappers;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Services
{
    public class SupplierPlatformServiceTests
    {
        private Mock<ISupplierApiClient> _supplierApiClientMock;
        private Mock<ILogger<SupplierPlatformService>> _loggerMock;
        private Mock<ISupplierApiSelector> _selctorMock;
        private SupplierConfiguration _supplierConfig;
        private Mock<ISupplierSelfTestHttpClient> _supplierSelfTestHttpClient;
        private Mock<IPollyRetryService> _pollyRetryService;
        private SupplierPlatformService _SupplierPlatformService;
        private Conference _testConference;
        private Mock<IFeatureToggles> _featureToggles;

        [SetUp]
        public void Setup()
        {
            _featureToggles = new Mock<IFeatureToggles>();
            _featureToggles.Setup(x => x.VodafoneIntegrationEnabled()).Returns(false);
            _supplierApiClientMock = new Mock<ISupplierApiClient>();
            _supplierConfig = new KinlyConfiguration()
            {
                CallbackUri = "CallbackUri", ApiUrl = "KinlyApiUrl"
            };
            _selctorMock = new Mock<ISupplierApiSelector>();
            _selctorMock.Setup(e => e.GetSupplierConfiguration()).Returns(_supplierConfig);
            _selctorMock.Setup(e => e.GetHttpClient()).Returns(_supplierApiClientMock.Object);
            _loggerMock = new Mock<ILogger<SupplierPlatformService>>();

            _supplierSelfTestHttpClient = new Mock<ISupplierSelfTestHttpClient>();
            _pollyRetryService = new Mock<IPollyRetryService>();
            
            _SupplierPlatformService = new SupplierPlatformService(
                _loggerMock.Object,
                _supplierSelfTestHttpClient.Object,
                _pollyRetryService.Object,
                _selctorMock.Object,
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
                    _SupplierPlatformService.BookVirtualCourtroomAsync(_testConference.Id, false, "",
                        new List<EndpointDto>()))
                .ErrorMessage.Should().Be($"Meeting room for conference {_testConference.Id} has already been booked");
        }

        [Test]
        public void Should_throw_supplier_api_exception_when_booking_courtroom()
        {
            _supplierApiClientMock
                .Setup(x => x.CreateHearingAsync(It.IsAny<CreateHearingParams>()))
                .ThrowsAsync(new SupplierApiException("", StatusCodes.Status500InternalServerError, "", null, It.IsAny<Exception>()));

            Assert.ThrowsAsync<SupplierApiException>(() =>
                _SupplierPlatformService.BookVirtualCourtroomAsync(_testConference.Id, false, "", new List<EndpointDto>()));
        }
        
        [Test]
        public async Task Should_return_meeting_room_when_booking_courtroom()
        {
            const bool audioRecordingRequired = false;
            const string ingestUrl = null;
            var endpoints = new List<EndpointDto>
            {
                new () {Id = Guid.NewGuid(), Pin = "1234", DisplayName = "one", SipAddress = "99191919"},
                new () {Id = Guid.NewGuid(), Pin = "5678", DisplayName = "two", SipAddress = "5385983832"}
            };
            
            var hearingParams = new CreateHearingParams
            {
                Virtual_courtroom_id = _testConference.Id.ToString(),
                Callback_uri = _supplierConfig.CallbackUri,
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
            
            _supplierApiClientMock
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
                    Uris = uris,
                    Telephone_conference_id = "12345678",
                    Virtual_courtroom_id = Guid.NewGuid()
                });

            var result = await _SupplierPlatformService.BookVirtualCourtroomAsync(_testConference.Id,
                audioRecordingRequired,
                ingestUrl,
                endpoints);

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
                param.Jvs_endpoint != null && param.Jvs_endpoint.Count == hearingParams.Jvs_endpoint.Count
            )), Times.Once);
        }

        [Test]
        public async Task Should_update_virtual_court_room()
        {
            _supplierApiClientMock.Setup(x => x.UpdateHearingAsync(It.IsAny<string>(), It.IsAny<UpdateHearingParams>()));

            var conferenceId = Guid.NewGuid();
            await _SupplierPlatformService.UpdateVirtualCourtRoomAsync(conferenceId, true, new List<EndpointDto>());
            
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

            var result = await _SupplierPlatformService.GetVirtualCourtRoomAsync(It.IsAny<Guid>());

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

            var result = await _SupplierPlatformService.GetVirtualCourtRoomAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }

        [Test]
        public void Should_throw_for_supplier_virtual_court_room_when_other_status()
        {
            var exception = new SupplierApiException("BadGateway", StatusCodes.Status502BadGateway, "", null, null);
            _supplierApiClientMock.Setup(x => x.GetHearingAsync(It.IsAny<string>())).Throws(exception);

            Assert.ThrowsAsync<SupplierApiException>(() => _SupplierPlatformService.GetVirtualCourtRoomAsync(It.IsAny<Guid>()));
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

            var result = await _SupplierPlatformService.GetTestCallScoreAsync(participantId);
            
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTestCallResult);
        }

        [Test]
        public async Task Should_delete_virtual_court_room()
        {
            var conferenceId = Guid.NewGuid();
            _supplierApiClientMock.Setup(x => x.DeleteHearingAsync(conferenceId.ToString()));

            await _SupplierPlatformService.DeleteVirtualCourtRoomAsync(conferenceId);
            
            _supplierApiClientMock.Verify(x => x.DeleteHearingAsync(conferenceId.ToString()), Times.Once);
        }

        [Test]
        public async Task should_start_hearing_with_automatic_layout_as_default()
        {
            var conferenceId = Guid.NewGuid();
            await _SupplierPlatformService.StartHearingAsync(conferenceId, It.IsAny<string>());
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
            var muteGuests = false;
            await _SupplierPlatformService.StartHearingAsync(conferenceId, It.IsAny<string>(), participantsToForceTransfer, layout, muteGuests);
            _supplierApiClientMock.Verify(
                x => x.StartAsync(conferenceId.ToString(),
                    It.Is<StartHearingRequest>(l => l.Hearing_layout == layout && l.Force_transfer_participant_ids.SequenceEqual(participantsToForceTransfer) && l.Mute_guests == muteGuests)), Times.Once);
        }
        
        [Test]
        public async Task should_pause_hearing()
        {
            var conferenceId = Guid.NewGuid();
            await _SupplierPlatformService.PauseHearingAsync(conferenceId);
            _supplierApiClientMock.Verify(x => x.PauseHearingAsync(conferenceId.ToString()), Times.Once);
        }
        
        [Test]
        public async Task should_end_hearing()
        {
            var conferenceId = Guid.NewGuid();
            await _SupplierPlatformService.EndHearingAsync(conferenceId);
            _supplierApiClientMock.Verify(x => x.EndHearingAsync(conferenceId.ToString()), Times.Once);
        }
        
        [Test]	
        public async Task should_suspend_hearing()	
        {	
            var conferenceId = Guid.NewGuid();	
            await _SupplierPlatformService.SuspendHearingAsync(conferenceId);	
            _supplierApiClientMock.Verify(x => x.TechnicalAssistanceAsync(conferenceId.ToString()), Times.Once);	
        }
    }
}
