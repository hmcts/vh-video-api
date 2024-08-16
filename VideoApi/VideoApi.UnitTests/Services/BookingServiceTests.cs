using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;

namespace VideoApi.UnitTests.Services;

public class BookingServiceTests
{
    protected Mock<ICommandHandler> CommandHandlerMock;
    
    [SetUp]
    public void TestInitialize()
    {
        var request = new BookNewConferenceRequestBuilder("Video Api Unit Test Hearing").WithJudge()
            .WithRepresentative()
            .WithIndividual()
            .WithRepresentative("Respondent")
            .WithIndividual("Respondent")
            .WithEndpoint("DisplayName1", "1234567890", "1234")
            .WithEndpoint("DisplayName2", "0987654321", "5678")
            .Build();
    }
    
    [Test]
    public async Task Should_verify_double_booking_for_given_conference_id()
    {
        var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true)
            { IngestUrl = "http://myIngestUrl.com" };
        SetupCallToMockRetryService(audioPlatformServiceResponse);
        VideoPlatformServiceMock
            .Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<EndpointDto>>())).Throws(new DoubleBookingException(Guid.NewGuid()));
        
        await Controller.BookMeetingRoomAsync(Guid.NewGuid(), true, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
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
            audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>())).ReturnsAsync(MeetingRoom);
        
        await Controller.BookMeetingRoomAsync(Guid.NewGuid(), true, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
        CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
    }
    
    [Test]
    public async Task Should_book_supplier_conference_with_ingesturl_when_audio_recording_not_required()
    {
        var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true)
            { IngestUrl = "http://myIngestUrl.com" };
        SetupCallToMockRetryService(audioPlatformServiceResponse);
        VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), false,
            audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>())).ReturnsAsync(MeetingRoom);
        
        await Controller.BookMeetingRoomAsync(Guid.NewGuid(), false, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), false, audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
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
                It.IsAny<IEnumerable<EndpointDto>>())).ReturnsAsync((MeetingRoom)null);
        
        await Controller.BookMeetingRoomAsync(Guid.NewGuid(), true, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
        CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Never);
    }
    
    [Test]
    public async Task Should_book_supplier_conference_with_ingesturl_when_audio_recording_is_required()
    {
        var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true)
            { IngestUrl = "http://myIngestUrl.com" };
        SetupCallToMockRetryService(audioPlatformServiceResponse);
        VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), true,
            audioPlatformServiceResponse.IngestUrl, It.IsAny<IEnumerable<EndpointDto>>())).ReturnsAsync(MeetingRoom);
        
        await Controller.BookMeetingRoomAsync(Guid.NewGuid(), true, audioPlatformServiceResponse.IngestUrl,
            new EndpointDto[] { });
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), true, audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
        CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
    }
    
    [Test]
    public async Task Should_book_supplier_conference_room_for_given_conference_id_with_endpoints()
    {
        var audioPlatformServiceResponse = new AudioPlatformServiceResponse(true)
            { IngestUrl = "http://myIngestUrl.com" };
        SetupCallToMockRetryService(audioPlatformServiceResponse);
        VideoPlatformServiceMock.Setup(v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(),
            It.IsAny<string>(), It.IsAny<IEnumerable<EndpointDto>>())).ReturnsAsync(MeetingRoom);
        
        var response = await Controller.BookMeetingRoomAsync(Guid.NewGuid(), true,
            audioPlatformServiceResponse.IngestUrl, new EndpointDto[] { });
        
        response.Should().BeTrue();
        
        VideoPlatformServiceMock.Verify(
            v => v.BookVirtualCourtroomAsync(It.IsAny<Guid>(), It.IsAny<bool>(), audioPlatformServiceResponse.IngestUrl,
                It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
        CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateMeetingRoomCommand>()), Times.Once);
    }
}
