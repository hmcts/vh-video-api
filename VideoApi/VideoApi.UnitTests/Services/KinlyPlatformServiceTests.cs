using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Security.Kinly;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Kinly;
using VideoApi.Services.Mappers;
using StartHearingRequest = VideoApi.Contract.Requests.StartHearingRequest;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Services
{
    public class KinlyPlatformServiceTests
    {
        private Mock<IKinlyApiClient> _kinlyApiClientMock;
        private Mock<ILogger<KinlyPlatformService>> _loggerMock;
        private IOptions<KinlyConfiguration> _kinlyConfigOptions;

        private Mock<IKinlySelfTestHttpClient> _kinlySelfTestHttpClient;
        private Mock<IPollyRetryService> _pollyRetryService;
        private KinlyPlatformService _kinlyPlatformService;
        private Conference _testConference;

        [SetUp]
        public void Setup()
        {
            _kinlyApiClientMock = new Mock<IKinlyApiClient>();
            _loggerMock = new Mock<ILogger<KinlyPlatformService>>();

            _kinlySelfTestHttpClient = new Mock<IKinlySelfTestHttpClient>();
            _pollyRetryService = new Mock<IPollyRetryService>();
            
            _kinlyConfigOptions = Options.Create(new KinlyConfiguration()
            {
                CallbackUri = "CallbackUri", KinlyApiUrl = "KinlyApiUrl"
            });

            _kinlyPlatformService = new KinlyPlatformService(
                _kinlyApiClientMock.Object,
                _kinlyConfigOptions,
                _loggerMock.Object,
                _kinlySelfTestHttpClient.Object,
                _pollyRetryService.Object
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
                Callback_uri = _kinlyConfigOptions.Value.CallbackUri,
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
                    Uris = uris,
                    Telephone_conference_id = "12345678",
                    Virtual_courtroom_id = Guid.NewGuid()
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
                },
                Telephone_conference_id = "12345678",
                Virtual_courtroom_id = Guid.NewGuid()
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
                x => x.StartAsync(conferenceId.ToString(),
                    It.Is<VideoApi.Services.Kinly.StartHearingRequest>(l => l.Hearing_layout == Layout.AUTOMATIC)), Times.Once);
        }
        
        [Test]
        public async Task should_start_hearing_with_provided_layout()
        {
            var conferenceId = Guid.NewGuid();
            var layout = Layout.ONE_PLUS_SEVEN;
            var participantsToForceTransfer = new[] {"participant-one", "participant-two"};
            var muteGuests = false;
            await _kinlyPlatformService.StartHearingAsync(conferenceId, participantsToForceTransfer, layout, muteGuests);
            _kinlyApiClientMock.Verify(
                x => x.StartAsync(conferenceId.ToString(),
                    It.Is<VideoApi.Services.Kinly.StartHearingRequest>(l => l.Hearing_layout == layout && l.Force_transfer_participant_ids.SequenceEqual(participantsToForceTransfer) && l.Mute_guests == muteGuests)), Times.Once);
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
        public async Task should_suspend_hearing()	
        {	
            var conferenceId = Guid.NewGuid();	
            await _kinlyPlatformService.SuspendHearingAsync(conferenceId);	
            _kinlyApiClientMock.Verify(x => x.TechnicalAssistanceAsync(conferenceId.ToString()), Times.Once);	
        }
    }
}
