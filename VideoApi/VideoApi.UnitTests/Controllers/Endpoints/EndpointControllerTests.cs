using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Controllers;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Mappers;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Endpoints
{
    public class EndpointControllerTests
    {
        private readonly Mock<ICommandHandler> _commandHandlerMock;
        private readonly Mock<IQueryHandler> _queryHandlerMock;
        private readonly Mock<IVideoPlatformService> _videoPlatformServiceMock;

        private readonly EndpointsController _controller;

        public EndpointControllerTests()
        {
            _queryHandlerMock = new Mock<IQueryHandler>();
            _commandHandlerMock = new Mock<ICommandHandler>();
            var mockLogger = new Mock<ILogger<EndpointsController>>();
            _videoPlatformServiceMock = new Mock<IVideoPlatformService>();

            _controller = new EndpointsController(_queryHandlerMock.Object,
                _commandHandlerMock.Object,
                _videoPlatformServiceMock.Object,
                mockLogger.Object);
        }

        [Test]
        public async Task Should_get_endpoints_for_conference()
        {
            var endpoints = new List<Endpoint>
            {
                new Endpoint("one", "sip@sip.com", "1234","Defence Sol")
            };

            _queryHandlerMock
                .Setup(x => x.Handle<GetEndpointsForConferenceQuery, IList<Endpoint>>(It.IsAny<GetEndpointsForConferenceQuery>()))
                .ReturnsAsync(endpoints);

            var response = await _controller.GetEndpointsForConference(Guid.NewGuid());

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<OkObjectResult>();
            var result = response.As<OkObjectResult>();
            result.Should().NotBeNull();
            result.Value.Should().NotBeNull().And.Subject.Should().BeAssignableTo<List<EndpointResponse>>();
            var responseEndpoints = (List<EndpointResponse>)result.Value;
            responseEndpoints.Should().HaveCount(endpoints.Count);
            responseEndpoints[0].DisplayName.Should().Be("one");
        }

        [Test]
        public async Task Should_add_endpoint_to_conference()
        {
            var conferenceId = Guid.NewGuid();
            var testEndpoints = new List<Endpoint>
            {
                new ("one", "44564", "1234","Defence Sol"),
                new ("two", "867744", "5678", "Defence Bol")
            };

            var testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithEndpoints(testEndpoints)
                .Build();

            _queryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>
                    (
                        It.Is<GetConferenceByIdQuery>(y => y.ConferenceId == conferenceId))
                )
                .ReturnsAsync(testConference);

            _videoPlatformServiceMock
                .Setup(x => x.UpdateVirtualCourtRoomAsync(testConference.Id,
                    testConference.AudioRecordingRequired,
                    testConference.GetEndpoints()
                        .Select(EndpointMapper.MapToEndpoint)))
                .Returns(Task.CompletedTask);

            var response = await _controller.AddEndpointToConference(conferenceId, new AddEndpointRequest());

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<NoContentResult>();
            ((NoContentResult) response).StatusCode.Should().Be((int) HttpStatusCode.NoContent);

            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<AddEndpointCommand>()), Times.Once);

            _videoPlatformServiceMock.Verify(x => x.UpdateVirtualCourtRoomAsync(testConference.Id,
                testConference.AudioRecordingRequired,
                It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
        }

        [Test]
        public async Task Should_remove_endpoint_from_conference()
        {
            var conferenceId = Guid.NewGuid();
            var testEndpoints = new List<Endpoint>
            {
                new Endpoint("one", "44564", "1234", "Defence Sol"),
                new Endpoint("two", "867744", "5678", "Defence Bol")
            };

            var testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithEndpoints(testEndpoints)
                .Build();

            _queryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>
                    (
                        It.Is<GetConferenceByIdQuery>(y => y.ConferenceId == conferenceId))
                )
                .ReturnsAsync(testConference);

            _videoPlatformServiceMock
                .Setup(x => x.UpdateVirtualCourtRoomAsync(testConference.Id,
                    testConference.AudioRecordingRequired,
                    testConference.GetEndpoints()
                        .Select(EndpointMapper.MapToEndpoint)))
                .Returns(Task.CompletedTask);

            var response = await _controller.RemoveEndpointFromConference(conferenceId, "sip@sip.com");

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<NoContentResult>();
            ((NoContentResult) response).StatusCode.Should().Be((int) HttpStatusCode.NoContent);

            _commandHandlerMock.Verify(x => x.Handle(It.Is<RemoveEndpointCommand>
            (
                y => y.ConferenceId == conferenceId && y.SipAddress == "sip@sip.com"
            )), Times.Once);

            _videoPlatformServiceMock.Verify(x => x.UpdateVirtualCourtRoomAsync(testConference.Id,
                testConference.AudioRecordingRequired,
                It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
        }

        [Test]
        public async Task Should_update_endpoint_in_conference()
        {
            const string newDisplayName = "new display name";
            var testEndpoints = new List<Endpoint>
            {
                new Endpoint("one", "44564", "1234", "Defence Sol"),
                new Endpoint("two", "867744", "5678", "Defence Bol")
            };

            var testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithEndpoints(testEndpoints)
                .Build();

            _queryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>
                    (
                        It.Is<GetConferenceByIdQuery>(y => y.ConferenceId == testConference.Id))
                )
                .ReturnsAsync(testConference);

            _videoPlatformServiceMock
                .Setup(x => x.UpdateVirtualCourtRoomAsync(testConference.Id,
                    testConference.AudioRecordingRequired,
                    testConference.GetEndpoints()
                        .Select(EndpointMapper.MapToEndpoint)))
                .Returns(Task.CompletedTask);

            var response = await _controller.UpdateEndpointInConference(testConference.Id, "sip@sip.com", new UpdateEndpointRequest
            {
                DisplayName = newDisplayName
            });

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<OkResult>();
            ((OkResult) response).StatusCode.Should().Be((int) HttpStatusCode.OK);

            _commandHandlerMock.Verify(x => x.Handle(It.Is<UpdateEndpointCommand>
            (
                y => y.ConferenceId == testConference.Id && y.SipAddress == "sip@sip.com" && y.DisplayName == newDisplayName
            )), Times.Once);

            _videoPlatformServiceMock.Verify(x => x.UpdateVirtualCourtRoomAsync(testConference.Id,
                testConference.AudioRecordingRequired,
                It.IsAny<IEnumerable<EndpointDto>>()), Times.Once);
        }

        [Test]
        public async Task should_not_update_kinly_when_endpoint_display_name_is_not_updated()
        {
            const string defenceAdvocate = "new defence advocate";
            var testEndpoints = new List<Endpoint>
            {
                new Endpoint("one", "44564", "1234", ("Defence Sol", LinkedParticipantType.DefenceAdvocate)),
                new Endpoint("two", "867744", "5678", ("Defence Bol", LinkedParticipantType.DefenceAdvocate))
            };

            var testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithEndpoints(testEndpoints)
                .Build();

            _queryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>
                    (It.Is<GetConferenceByIdQuery>(y => y.ConferenceId == testConference.Id)))
                .ReturnsAsync(testConference);

            _videoPlatformServiceMock
                .Setup(x => x.UpdateVirtualCourtRoomAsync(testConference.Id,
                    testConference.AudioRecordingRequired,
                    testConference.GetEndpoints()
                        .Select(EndpointMapper.MapToEndpoint)))
                .Returns(Task.CompletedTask);

            var endpointParticipants = new [] { new EndpointParticipantRequest { ParticipantUsername = defenceAdvocate, Type = Contract.Enums.LinkedParticipantType.DefenceAdvocate } };
            var response = await _controller.UpdateEndpointInConference(
                testConference.Id, 
                "sip@sip.com", 
                new UpdateEndpointRequest
                {
                    EndpointParticipants = endpointParticipants
                });

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<OkResult>();
            ((OkResult) response).StatusCode.Should().Be((int) HttpStatusCode.OK);
            _commandHandlerMock.Verify(x => x.Handle(It.Is<UpdateEndpointCommand>
            (
                y => y.ConferenceId == testConference.Id && 
                      y.SipAddress == "sip@sip.com" && 
                      y.DisplayName == null &&
                      endpointParticipants.Map()[0].Equals(y.EndpointParticipants[0]))
            ), Times.Once);

            _videoPlatformServiceMock.Verify(x => x.UpdateVirtualCourtRoomAsync(testConference.Id,
                testConference.AudioRecordingRequired,
                It.IsAny<IEnumerable<EndpointDto>>()), Times.Never);
            
        }
    }
}
