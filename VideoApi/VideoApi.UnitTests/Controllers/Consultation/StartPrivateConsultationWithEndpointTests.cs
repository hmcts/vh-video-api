using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Testing.Common.Assertions;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation;

public class StartPrivateConsultationWithEndpointTests : ConsultationControllerTestBase
{
    [Test]
    public async Task should_return_not_found_when_conference_not_found()
    {
        var conferenceId = Guid.NewGuid();
        var endpointId = TestConference.GetEndpoints()[0].Id;
        
        var request = new EndpointConsultationRequest
        {
            ConferenceId = conferenceId,
            EndpointId = endpointId
        };
        var result = await Controller.StartConsultationWithEndpointAsync(request);
        
        var actionResult = result.As<NotFoundObjectResult>();
        actionResult.Should().NotBeNull();
        actionResult.Value.Should().Be($"Unable to find conference {request.ConferenceId}");
    }
    
    [Test]
    public async Task should_return_not_found_when_endpoint_not_found()
    {
        var conferenceId = TestConference.Id;
        var endpointId = Guid.NewGuid();
        
        var request = new EndpointConsultationRequest
        {
            ConferenceId = conferenceId,
            EndpointId = endpointId
        };
        var result = await Controller.StartConsultationWithEndpointAsync(request);
        
        var actionResult = result.As<NotFoundObjectResult>();
        actionResult.Should().NotBeNull();
        actionResult.Value.Should().Be($"Unable to find endpoint {request.EndpointId}");
    }
    
    [Test]
    public async Task should_return_bad_request_when_endpoint_is_already_in_room()
    {
        var endpoint = TestConference.GetEndpoints()[0];
        var participant = TestConference.GetParticipants()[0];
        
        var room = new ConsultationRoom(TestConference.Id, "Label", VideoApi.Domain.Enums.VirtualCourtRoomType.Participant, false);
        room.AddEndpoint(new RoomEndpoint(Guid.NewGuid()));
        QueryHandlerMock.Setup(x => x.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(It.IsAny<GetConsultationRoomByIdQuery>())).ReturnsAsync(room);
        
        var request = new EndpointConsultationRequest()
        {
            ConferenceId = TestConference.Id,
            EndpointId = endpoint.Id,
            RequestedById = participant.Id,
            RoomLabel = "Label"
        };
        var result = await Controller.StartConsultationWithEndpointAsync(request);
        
        result.Should().NotBeNull();
        var objectResult = (ObjectResult)result;
        ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage("RoomLabel", "Room already has an active endpoint");
    }
    
    [Test]
    public async Task should_return_not_found_when_endpoint_is_requested_to_not_found_room()
    {
        var endpoint = TestConference.GetEndpoints()[0];
        var participant = TestConference.GetParticipants()[0];
        
        QueryHandlerMock.Setup(x => x.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(It.IsAny<GetConsultationRoomByIdQuery>())).ReturnsAsync(null as ConsultationRoom);
        
        var request = new EndpointConsultationRequest()
        {
            ConferenceId = TestConference.Id,
            EndpointId = endpoint.Id,
            RequestedById = participant.Id,
            RoomLabel = "Label"
        };
        var result = await Controller.StartConsultationWithEndpointAsync(request);
        
        var actionResult = result.As<NotFoundObjectResult>();
        actionResult.Should().NotBeNull();
        actionResult.Value.Should().Be($"Unable to find room {request.RoomLabel}");
    }
    
    [Test]
    public async Task should_return_ok()
    {
        var endpointWithDefenceAdvocate = TestConference.GetEndpoints()[0];
        var defenceAdvocate = TestConference.GetParticipants()[0];
        
        var room = new ConsultationRoom(TestConference.Id, "Label", VideoApi.Domain.Enums.VirtualCourtRoomType.Participant, false);
        QueryHandlerMock.Setup(x => x.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(It.IsAny<GetConsultationRoomByIdQuery>())).ReturnsAsync(room);
        
        var request = new EndpointConsultationRequest
        {
            ConferenceId = TestConference.Id,
            EndpointId = endpointWithDefenceAdvocate.Id,
            RequestedById = defenceAdvocate.Id,
            RoomLabel = "Label"
        };
        var result = await Controller.StartConsultationWithEndpointAsync(request);
        
        result.Should().BeOfType<OkResult>();
    }
}
