using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.Services.Dtos;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class BookNewConferenceTests : ConferenceControllerTestBase
    {
        private BookNewConferenceRequest _request;
        
        [SetUp]
        public void TestInitialize()
        {
            _request = new BookNewConferenceRequestBuilder("Video Api Unit Test Hearing")
                .WithJudge()
                .WithRepresentative().WithIndividual()
                .WithRepresentative("Respondent").WithIndividual("Respondent")
                .WithEndpoint("DisplayName1", "1234567890", "1234")
                .WithEndpoint("DisplayName2", "0987654321", "5678")
                .Build();
        }
        
        [Test]
        public async Task Should_book_supplier_conference_room_for_given_conference_id_retries()
        {
            SetupCallToMockRetryService(new AudioPlatformServiceResponse(true)
                { IngestUrl = "http://myIngestUrl.com" });
            SetupCallToMockRetryService(Guid.NewGuid());
            SetupCallToMockRetryService(true);
            
            await Controller.BookNewConferenceAsync(_request);
            
            QueryHandlerMock.Verify(
                q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()),
                Times.Once);
        }
        
        
        [Test]
        public async Task Should_book_supplier_conference_with_ingesturl_when_audio_recording_not_required_retries()
        {
            SetupCallToMockRetryService(new AudioPlatformServiceResponse(true)
                { IngestUrl = "http://myIngestUrl.com" });
            SetupCallToMockRetryService(Guid.NewGuid());
            SetupCallToMockRetryService(true);
            
            _request.AudioRecordingRequired = false;
            await Controller.BookNewConferenceAsync(_request);
            
            QueryHandlerMock.Verify(
                q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()),
                Times.Once);
        }
        
        [Test]
        public async Task Should_book_supplier_conference_with_ingesturl_when_audio_recording_is_required_retries()
        {
            SetupCallToMockRetryService(new AudioPlatformServiceResponse(true)
                { IngestUrl = "http://myIngestUrl.com" });
            SetupCallToMockRetryService(Guid.NewGuid());
            SetupCallToMockRetryService(true);
            
            _request.AudioRecordingRequired = true;
            await Controller.BookNewConferenceAsync(_request);
            
            QueryHandlerMock.Verify(
                q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()),
                Times.Once);
        }
        
        [Test]
        public async Task Should_book_supplier_conference_with_ingest_url_when_hrs_integration_feature_is_enabled()
        {
            var expectedIngestUrl =
                $"https://localhost.streaming.mediaServices.windows.net/{_request.CaseTypeServiceId}-{_request.CaseNumber}-{_request.HearingRefId.ToString()}";
            AudioPlatformServiceMock
                .Setup(x => x.GetAudioIngestUrl(_request.CaseTypeServiceId, _request.CaseNumber,
                    _request.HearingRefId.ToString())).Returns(expectedIngestUrl);
            SetupCallToMockRetryService(new AudioPlatformServiceResponse(true) { IngestUrl = expectedIngestUrl });
            SetupCallToMockRetryService(Guid.NewGuid());
            SetupCallToMockRetryService(true);
            
            await Controller.BookNewConferenceAsync(_request);
            
            BookingServiceMock.Verify(v
                => v.BookMeetingRoomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), expectedIngestUrl,
                    It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<ConferenceRoomType>(), It.IsAny<Supplier>()), Times.Once);
        }
        
        [Test]
        public async Task Should_return_error_when_creating_audio_application()
        {
            SetupCallToMockRetryService(new AudioPlatformServiceResponse(false));
            
            var response = await Controller.BookNewConferenceAsync(_request) as ActionResult;
            
            response.Should().NotBeNull();
            response.Should().BeAssignableTo<ObjectResult>();
            
            QueryHandlerMock.Verify(
                q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()),
                Times.Never);
        }
        
        [Test]
        public async Task Should_return_500_when_error_saving_conference()
        {
            SetupCallToMockRetryService(new AudioPlatformServiceResponse(true)
                { IngestUrl = "http://myIngestUrl.com" });
            SetupCallToMockRetryService(Guid.Empty);
            
            var response = await Controller.BookNewConferenceAsync(_request) as ActionResult;
            
            response.Should().NotBeNull();
            response.Should().BeAssignableTo<ObjectResult>();
            
            QueryHandlerMock.Verify(
                q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()),
                Times.Never);
        }
        
        [Test]
        public async Task Should_return_500_when_error_saving_booking_meeting_room_supplier()
        {
            SetupCallToMockRetryService(new AudioPlatformServiceResponse(true)
                { IngestUrl = "http://myIngestUrl.com" });
            SetupCallToMockRetryService(Guid.NewGuid());
            SetupCallToMockRetryService(false);
            
            var response = await Controller.BookNewConferenceAsync(_request) as ActionResult;
            
            response.Should().NotBeNull();
            response.Should().BeAssignableTo<ObjectResult>();
            
            QueryHandlerMock.Verify(
                q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()),
                Times.Never);
        }
        
        [Test]
        public async Task Should_save_all_successfully()
        {
            SetupCallToMockRetryService(new AudioPlatformServiceResponse(true)
                { IngestUrl = "http://myIngestUrl.com" });
            SetupCallToMockRetryService(Guid.NewGuid());
            SetupCallToMockRetryService(true);
            
            var response = await Controller.BookNewConferenceAsync(_request) as ActionResult;
            
            response.Should().NotBeNull();
            response.Should().BeAssignableTo<ObjectResult>();
            
            QueryHandlerMock.Verify(
                q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()),
                Times.Once);
        }
        
        [TestCase(Supplier.Kinly)]
        [TestCase(Supplier.Vodafone)]
        public async Task Should_book_supplier_conference_with_requested_supplier(Supplier supplier)
        {
            SetupCallToMockRetryService(new AudioPlatformServiceResponse(true)
                { IngestUrl = "http://myIngestUrl.com" });
            SetupCallToMockRetryService(Guid.NewGuid());
            SetupCallToMockRetryService(true);
            _request.Supplier = supplier;
            TestConference.UpdateMeetingRoom("adminUri", "judgeUri", "participantUri", "pexipNode",
                "telephoneConferenceId");
            UseSupplierPlatformServiceStub();
            
            var response = await Controller.BookNewConferenceAsync(_request) as ActionResult;
            
            response.Should().NotBeNull();
            response.Should().BeAssignableTo<CreatedAtActionResult>();
            var result = ((CreatedAtActionResult)response).Value as ConferenceDetailsResponse;
            
            VerifySupplierUsed(supplier, Times.Exactly(1));
            result.MeetingRoom.Should().NotBeNull();
            switch (supplier)
            {
                case Supplier.Kinly:
                    result.MeetingRoom.PexipSelfTestNode.Should().Be(KinlyConfig.PexipSelfTestNode);
                    break;
                case Supplier.Vodafone:
                    result.MeetingRoom.PexipSelfTestNode.Should().Be(VodafoneConfig.PexipSelfTestNode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(supplier), supplier, null);
            }
        }
    }
}
