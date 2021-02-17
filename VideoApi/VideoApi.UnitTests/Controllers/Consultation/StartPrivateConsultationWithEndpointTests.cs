using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
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
                DefenceAdvocateId = defenceAdvocateId
            };
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            var actionResult = result.As<NotFoundObjectResult>();
            actionResult.Should().NotBeNull();
            actionResult.Value.Should().Be($"Unable to find defence advocate {request.DefenceAdvocateId}");
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
                DefenceAdvocateId = defenceAdvocate.Id
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
                    StringComparison.CurrentCultureIgnoreCase));
            
            var request = new EndpointConsultationRequest()
            {
                ConferenceId = TestConference.Id,
                EndpointId = endpointWithDefenceAdvocate.Id,
                DefenceAdvocateId = defenceAdvocate.Id
            };
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            var actionResult = result.As<UnauthorizedObjectResult>();
            actionResult.Should().NotBeNull();
            actionResult.Value.Should().Be("Defence advocate is not allowed to speak to requested endpoint");
        }
        
        [Test]
        public async Task should_return_ok_when_endpoint_is_linked_with_defence_advocate()
        {
            var endpointWithDefenceAdvocate = TestConference.GetEndpoints().First(x => !string.IsNullOrWhiteSpace(x.DefenceAdvocate));
            var defenceAdvocate = TestConference.GetParticipants().First(x =>
                x.Username.Equals(endpointWithDefenceAdvocate.DefenceAdvocate,
                    StringComparison.CurrentCultureIgnoreCase));

            var room = new Room(TestConference.Id, "Label", VideoApi.Domain.Enums.VirtualCourtRoomType.Participant, false);
            ConsultationServiceMock.Setup(x => x.CreateNewConsultationRoomAsync(TestConference.Id, VideoApi.Domain.Enums.VirtualCourtRoomType.Participant, false)).ReturnsAsync(room);


            var request = new EndpointConsultationRequest()
            {
                ConferenceId = TestConference.Id,
                EndpointId = endpointWithDefenceAdvocate.Id,
                DefenceAdvocateId = defenceAdvocate.Id
            };
            var result = await Controller.StartConsultationWithEndpointAsync(request);

            result.Should().BeOfType<OkResult>();
        }
    }
}
