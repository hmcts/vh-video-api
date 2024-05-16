using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class StartPrivateConsultationWithEndpointTests : ConsultationControllerTestBase
    {
        [Test]
        public async Task should_return_not_found_when_conference_not_found()
        {
            var conferenceId = Guid.NewGuid();
            var endpointId = TestConference.GetEndpoints().First().Id;

            var request = new EndpointConsultationRequest()
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

            var request = new EndpointConsultationRequest()
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
        public async Task should_return_not_found_when_defence_advocate_not_found()
        {
            var conferenceId = TestConference.Id;
            var endpointId = TestConference.GetEndpoints().First().Id;
            var defenceAdvocateId = Guid.NewGuid();
            
            var request = new EndpointConsultationRequest()
            {
                ConferenceId = conferenceId,
                EndpointId = endpointId,
                RequestedById = defenceAdvocateId
            };
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            var actionResult = result.As<NotFoundObjectResult>();
            actionResult.Should().NotBeNull();
            actionResult.Value.Should().Be($"Unable to find defence advocate {request.RequestedById}");
        }
        
        [Test]
        public async Task should_return_unauthorised_when_endpoint_does_not_have_defence_advocate()
        {
            var endpointWithDefenceAdvocate = TestConference.GetEndpoints().First(x => !string.IsNullOrWhiteSpace(x.DefenceAdvocate));
            var endpointWithoutDefenceAdvocate = TestConference.GetEndpoints().First(x => string.IsNullOrWhiteSpace(x.DefenceAdvocate));
            var defenceAdvocate = TestConference.GetParticipants().First(x =>
                x.Username.Equals(endpointWithDefenceAdvocate.DefenceAdvocate,
                    StringComparison.CurrentCultureIgnoreCase));
            
            var request = new EndpointConsultationRequest()
            {
                ConferenceId = TestConference.Id,
                EndpointId = endpointWithoutDefenceAdvocate.Id,
                RequestedById = defenceAdvocate.Id
            };
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            var actionResult = result.As<UnauthorizedObjectResult>();
            actionResult.Should().NotBeNull();
            actionResult.Value.Should().Be("Endpoint does not have a defence advocate linked");
        }
        
        [Test]
        public async Task should_return_unauthorised_when_endpoint_is_not_linked_with_defence_advocate()
        {
            var endpointWithDefenceAdvocate = TestConference.GetEndpoints().First(x => !string.IsNullOrWhiteSpace(x.DefenceAdvocate));
            var defenceAdvocate = TestConference.GetParticipants().First(x =>
                !x.Username.Equals(endpointWithDefenceAdvocate.DefenceAdvocate,
                    StringComparison.CurrentCultureIgnoreCase) && x.UserRole != VideoApi.Domain.Enums.UserRole.Judge);
            
            var request = new EndpointConsultationRequest()
            {
                ConferenceId = TestConference.Id,
                EndpointId = endpointWithDefenceAdvocate.Id,
                RequestedById = defenceAdvocate.Id
            };
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            var actionResult = result.As<UnauthorizedObjectResult>();
            actionResult.Should().NotBeNull();
            actionResult.Value.Should().Be("Defence advocate is not allowed to speak to requested endpoint");
        }

        [Test]
        public async Task should_return_bad_request_when_endpoint_is_already_in_room()
        {
            var endpointWithDefenceAdvocate = TestConference.GetEndpoints().First(x => !string.IsNullOrWhiteSpace(x.GetDefenceAdvocate()));
            var defenceAdvocate = TestConference.GetParticipants().First(x =>
                x.Username.Equals(endpointWithDefenceAdvocate.GetDefenceAdvocate(), StringComparison.CurrentCultureIgnoreCase));

            var room = new ConsultationRoom(TestConference.Id, "Label", VideoApi.Domain.Enums.VirtualCourtRoomType.Participant, false);
            room.AddEndpoint(new RoomEndpoint(Guid.NewGuid()));
            QueryHandlerMock.Setup(x => x.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(It.IsAny<GetConsultationRoomByIdQuery>())).ReturnsAsync(room);

            var request = new EndpointConsultationRequest
            {
                ConferenceId = TestConference.Id,
                EndpointId = endpointWithDefenceAdvocate.Id,
                RequestedById = defenceAdvocate.Id,
                RoomLabel = "Label"
            };
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            var actionResult = result.As<BadRequestObjectResult>();
            actionResult.Should().NotBeNull();
            actionResult.Value.Should().Be("Room already has an active endpoint");
        }

        [Test]
        public async Task should_return_not_found_when_endpoint_is_requested_to_not_found_room()
        {
            var endpointWithDefenceAdvocate = TestConference.GetEndpoints().First(x => !string.IsNullOrWhiteSpace(x.GetDefenceAdvocate()));
            var defenceAdvocate = TestConference.GetParticipants().First(x =>
                x.Username.Equals(endpointWithDefenceAdvocate.GetDefenceAdvocate(),
                    StringComparison.CurrentCultureIgnoreCase));

            QueryHandlerMock.Setup(x => x.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(It.IsAny<GetConsultationRoomByIdQuery>())).ReturnsAsync(null as ConsultationRoom);

            var request = new EndpointConsultationRequest()
            {
                ConferenceId = TestConference.Id,
                EndpointId = endpointWithDefenceAdvocate.Id,
                RequestedById = defenceAdvocate.Id,
                RoomLabel = "Label"
            };
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            var actionResult = result.As<NotFoundObjectResult>();
            actionResult.Should().NotBeNull();
            actionResult.Value.Should().Be($"Unable to find room {request.RoomLabel}");
        }

        [Test]
        public async Task should_return_ok_when_endpoint_is_linked_with_defence_advocate()
        {
            var endpointWithDefenceAdvocate = TestConference.GetEndpoints().First(x => !string.IsNullOrWhiteSpace(x.GetDefenceAdvocate()));
            var defenceAdvocate = TestConference.GetParticipants().First(x =>
                x.Username.Equals(endpointWithDefenceAdvocate.GetDefenceAdvocate(),
                    StringComparison.CurrentCultureIgnoreCase));

            var room = new ConsultationRoom(TestConference.Id, "Label", VideoApi.Domain.Enums.VirtualCourtRoomType.Participant, false);
            QueryHandlerMock.Setup(x => x.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(It.IsAny<GetConsultationRoomByIdQuery>())).ReturnsAsync(room);

            var request = new EndpointConsultationRequest()
            {
                ConferenceId = TestConference.Id,
                EndpointId = endpointWithDefenceAdvocate.Id,
                RequestedById = defenceAdvocate.Id,
                RoomLabel = "Label"
            };
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task should_return_ok_when_vho_invites()
        {
            // Arrange
            var endpointWithoutDefenceAdvocate = TestConference.GetEndpoints().First(x => string.IsNullOrWhiteSpace(x.GetDefenceAdvocate()));
            var request = new EndpointConsultationRequest()
            {
                ConferenceId = TestConference.Id,
                EndpointId = endpointWithoutDefenceAdvocate.Id,
                RequestedById = Guid.Empty,
                RoomLabel = "NewRoom_NotInDb"
            };

            // Act
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task should_return_ok_when_judge_invites()
        {
            // Arrange
            var endpointWithoutDefenceAdvocate = TestConference.GetEndpoints().First(x => string.IsNullOrWhiteSpace(x.GetDefenceAdvocate()));
            var requestedByJudge = TestConference.GetParticipants().First(x => x.UserRole == VideoApi.Domain.Enums.UserRole.Judge);
            var request = new EndpointConsultationRequest()
            {
                ConferenceId = TestConference.Id,
                EndpointId = endpointWithoutDefenceAdvocate.Id,
                RequestedById = requestedByJudge.Id,
                RoomLabel = "NewRoom_NotInDb"
            };

            // Act
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task should_return_ok_when_staff_member_invites()
        {
            // Arrange
            var endpointWithoutDefenceAdvocate = TestConference.GetEndpoints().First(x => string.IsNullOrWhiteSpace(x.GetDefenceAdvocate()));
            var requestedByJudge = TestConference.GetParticipants().First(x => x.UserRole == VideoApi.Domain.Enums.UserRole.StaffMember);
            var request = new EndpointConsultationRequest()
            {
                ConferenceId = TestConference.Id,
                EndpointId = endpointWithoutDefenceAdvocate.Id,
                RequestedById = requestedByJudge.Id,
                RoomLabel = "NewRoom_NotInDb"
            };

            // Act
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            // Assert
            result.Should().BeOfType<OkResult>();
        }
    }
}
