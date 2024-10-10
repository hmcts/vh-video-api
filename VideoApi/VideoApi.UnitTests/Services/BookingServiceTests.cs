using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Services;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using VideoApi.UnitTests.Controllers.Conference;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Services;

public class BookingServiceTests : ConferenceControllerTestBase
{
    private readonly Mock<ILogger<BookingService>> _logger = new();
    private BookingService _service;
    
    [SetUp]
    public void TestInitialize()
    {
        _service = new BookingService(SupplierPlatformServiceFactoryMock.Object, CommandHandlerMock.Object,
            QueryHandlerMock.Object, _logger.Object);
        
        QueryHandlerMock.
            Setup(x =>
                x.Handle<GetConferencesByTelephoneIdQuery, List<Conference>>(
                    It.IsAny<GetConferencesByTelephoneIdQuery>()))
            .ReturnsAsync(new List<Conference>());
    }
    
    [Test]
    public async Task Should_verify_double_booking_for_given_conference_id()
    {
        var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true)
            { IngestUrl = "http://myIngestUrl.com" };
        SetupCallToMockRetryService(audioPlatformServiceResponse);
        VideoPlatformServiceMock
            .Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>())).Throws(new DoubleBookingException(Guid.NewGuid()));
        
        await _service.BookMeetingRoomAsync(Guid.NewGuid(), true, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>()), Times.Once);
        VideoPlatformServiceMock.Verify(v => v.GetVirtualCourtRoomAsync(It.IsAny<Guid>()), Times.Once);
        CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Never);
    }
    
    [Test]
    public async Task Should_book_supplier_conference_and_update_meeting_room_for_given_conference_id()
    {
        var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true)
            { IngestUrl = "http://myIngestUrl.com" };
        SetupCallToMockRetryService(audioPlatformServiceResponse);
        VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(),
            audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>())).ReturnsAsync(MeetingRoom);
        
        await _service.BookMeetingRoomAsync(Guid.NewGuid(), true, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>()), Times.Once);
        CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
    }
    
    [Test]
    public async Task Should_book_supplier_conference_with_ingesturl_when_audio_recording_not_required()
    {
        var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true)
            { IngestUrl = "http://myIngestUrl.com" };
        SetupCallToMockRetryService(audioPlatformServiceResponse);
        VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), false,
            audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>())).ReturnsAsync(MeetingRoom);
        
        await _service.BookMeetingRoomAsync(Guid.NewGuid(), false, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), false, audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>()), Times.Once);
        CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
    }
    
    [Test]
    public async Task Should_book_supplier_conference_room_for_given_conference_id()
    {
        var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true)
            { IngestUrl = "http://myIngestUrl.com" };
        SetupCallToMockRetryService(audioPlatformServiceResponse);
        VideoPlatformServiceMock
            .Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>())).ReturnsAsync((MeetingRoom)null);
        
        await _service.BookMeetingRoomAsync(Guid.NewGuid(), true, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>()), Times.Once);
        CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Never);
    }
    
    [Test]
    public async Task Should_book_supplier_conference_with_ingesturl_when_audio_recording_is_required()
    {
        var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true)
            { IngestUrl = "http://myIngestUrl.com" };
        SetupCallToMockRetryService(audioPlatformServiceResponse);
        VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), true,
            audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>())).ReturnsAsync(MeetingRoom);
        
        await _service.BookMeetingRoomAsync(Guid.NewGuid(), true, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), true, audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>()), Times.Once);
        CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
    }
    
    [Test]
    public async Task Should_book_supplier_conference_room_for_given_conference_id_with_endpoints()
    {
        var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true)
            { IngestUrl = "http://myIngestUrl.com" };
        SetupCallToMockRetryService(audioPlatformServiceResponse);
        VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(),
            It.IsAny<string>(), It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>())).ReturnsAsync(MeetingRoom);
        
        var response = await _service.BookMeetingRoomAsync(Guid.NewGuid(), true, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        response.Should().BeTrue();
        
        VideoPlatformServiceMock.Verify(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(),
            audioPlatformServiceResponse.IngestUrl,
            It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>()), Times.Once);
        CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
    }
    
    [Test]
    public async Task Should_book_supplier_conference_room_but_regenerate_telephone_id_that_already_exists()
    {
        //Conference returned from query handler then no conference on second query
        QueryHandlerMock.
            SetupSequence(x =>
                x.Handle<GetConferencesByTelephoneIdQuery, List<Conference>>(
                    It.IsAny<GetConferencesByTelephoneIdQuery>()))
            .ReturnsAsync(new List<Conference>(){It.IsAny<Conference>()})
            .ReturnsAsync(new List<Conference>());
        
        var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true)
            { IngestUrl = "http://myIngestUrl.com" };
        SetupCallToMockRetryService(audioPlatformServiceResponse);
        VideoPlatformServiceMock
            .Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>())).ReturnsAsync((MeetingRoom)null);
        
        await _service.BookMeetingRoomAsync(Guid.NewGuid(), true, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<string>(), It.IsAny<ScreeningRoomType>()), Times.Once);
        CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Never);
        QueryHandlerMock.Verify(
            x => x.Handle<GetConferencesByTelephoneIdQuery, List<Conference>>(
                It.IsAny<GetConferencesByTelephoneIdQuery>()), Times.Exactly(2));
    }
}
