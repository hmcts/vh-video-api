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
using VideoApi.Services;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Mappers;
using RoomType = VideoApi.Domain.Enums.RoomType;
using Task = System.Threading.Tasks.Task;
using UserRole = VideoApi.Domain.Enums.UserRole;
using Supplier = VideoApi.Contract.Enums.Supplier;

namespace VideoApi.UnitTests.Controllers.Endpoints
{
    public class EndpointControllerTests
    {
        private readonly Mock<ICommandHandler> _commandHandlerMock;

        private readonly EndpointsController _controller;
        private readonly Mock<IQueryHandler> _queryHandlerMock;
        private readonly Mock<ISupplierPlatformServiceFactory> _supplierPlatformServiceFactoryMock;
        private readonly Mock<IVideoPlatformService> _videoPlatformServiceMock;

        public EndpointControllerTests()
        {
            _queryHandlerMock = new Mock<IQueryHandler>();
            _commandHandlerMock = new Mock<ICommandHandler>();
            var mockLogger = new Mock<ILogger<EndpointsController>>();
            _videoPlatformServiceMock = new Mock<IVideoPlatformService>();
            _supplierPlatformServiceFactoryMock = new Mock<ISupplierPlatformServiceFactory>();
            _supplierPlatformServiceFactoryMock.Setup(x => x.Create(It.IsAny<VideoApi.Domain.Enums.Supplier>())).Returns(_videoPlatformServiceMock.Object);

            _controller = new EndpointsController(_queryHandlerMock.Object,
                _commandHandlerMock.Object,
                _supplierPlatformServiceFactoryMock.Object,
                mockLogger.Object);
        }

        [Test]
        public async Task Should_get_endpoints_for_conference()
        {
            var endpoints = new List<Endpoint>
            {
                new ("one", "sip@sip.com", "1234")
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
            var testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithEndpoint("two", "867744", true)
                .WithEndpoint("one", "44564", true)
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
                        .Select(EndpointMapper.MapToEndpoint), testConference.ConferenceRoomType, testConference.AudioPlaybackLanguage))
                .Returns(Task.CompletedTask);

            var response = await _controller.AddEndpointToConference(conferenceId, new AddEndpointRequest());

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<NoContentResult>();
            ((NoContentResult) response).StatusCode.Should().Be((int) HttpStatusCode.NoContent);

            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<AddEndpointCommand>()), Times.Once);

            _videoPlatformServiceMock.Verify(x => x.UpdateVirtualCourtRoomAsync(testConference.Id,
                testConference.AudioRecordingRequired,
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<ConferenceRoomType>(), It.IsAny<AudioPlaybackLanguage>()), Times.Once);
            VerifySupplierUsed(testConference.Supplier, Times.Exactly(1));
        }

        [Test]
        public async Task Should_remove_endpoint_from_conference()
        {
            var conferenceId = Guid.NewGuid();

            var testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithEndpoint("two", "867744", true)
                .WithEndpoint("one", "44564", true)
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
                        .Select(EndpointMapper.MapToEndpoint), It.IsAny<ConferenceRoomType>(), It.IsAny<AudioPlaybackLanguage>()))
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
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<ConferenceRoomType>(), It.IsAny<AudioPlaybackLanguage>()), Times.Once);
            VerifySupplierUsed(testConference.Supplier, Times.Exactly(2));
        }

        [Test]
        public async Task Should_update_endpoint_in_conference()
        {
            const string newDisplayName = "new display name";
            var testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithEndpoint("two", "867744", true)
                .WithEndpoint("one", "44564", true)
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
                        .Select(EndpointMapper.MapToEndpoint), It.IsAny<ConferenceRoomType>(), It.IsAny<AudioPlaybackLanguage>()))
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
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<ConferenceRoomType>(), It.IsAny<AudioPlaybackLanguage>()), Times.Once);
            VerifySupplierUsed(testConference.Supplier, Times.Exactly(3));
        }

        [Test]
        public async Task should_not_update_supplier_when_endpoint_display_name_is_not_updated()
        {
            var testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithEndpoint("two", "867744", true)
                .WithEndpoint("one", "44564", true)
                .Build();
            
            var defenceAdvocate = testConference.GetParticipants().First(x => x.UserRole == UserRole.Representative);

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
                        .Select(EndpointMapper.MapToEndpoint), It.IsAny<ConferenceRoomType>(), It.IsAny<AudioPlaybackLanguage>()))
                .Returns(Task.CompletedTask);

            var response = await _controller.UpdateEndpointInConference(testConference.Id, "sip@sip.com", new UpdateEndpointRequest
            {
                ParticipantsLinked = [defenceAdvocate.Username]
            });

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<OkResult>();
            ((OkResult) response).StatusCode.Should().Be((int) HttpStatusCode.OK);

            _commandHandlerMock.Verify(x => x.Handle(It.Is<UpdateEndpointCommand>
            (
                y => y.ConferenceId == testConference.Id && y.SipAddress == "sip@sip.com" && y.ParticipantsLinked.Contains(defenceAdvocate.Username)
            )), Times.Once);

            _videoPlatformServiceMock.Verify(x => x.UpdateVirtualCourtRoomAsync(testConference.Id,
                testConference.AudioRecordingRequired,
                It.IsAny<IEnumerable<EndpointDto>>(), It.IsAny<ConferenceRoomType>(), It.IsAny<AudioPlaybackLanguage>()), Times.Never);
        }

        protected void VerifySupplierUsed(Supplier supplier, Times times)
        {
            VerifySupplierUsed((VideoApi.Domain.Enums.Supplier)supplier, times);
        }

        protected void VerifySupplierUsed(VideoApi.Domain.Enums.Supplier supplier, Times times)
        {
            _supplierPlatformServiceFactoryMock.Verify(x => x.Create(supplier), times);
        }
    }
}
