using System;
using System.Net;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.HealthChecks
{
    public class HealthTests
    {
        private HealthCheckController _controller;
        private Mock<IQueryHandler> _mockQueryHandler;
        private Mock<IVideoPlatformService> _mockVideoPlatformService;

        [SetUp]
        public void Setup()
        {
            _mockQueryHandler = new Mock<IQueryHandler>();
            _mockVideoPlatformService = new Mock<IVideoPlatformService>();
        }

        [Test]
        public async Task Should_return_ok_result_when_database_is_connected()
        {
            var hearingId = Guid.NewGuid();
            var conference = new ConferenceBuilder().Build();
            var query = new GetConferenceByIdQuery(hearingId);

            _controller = new HealthCheckController(_mockQueryHandler.Object, _mockVideoPlatformService.Object);
            _mockQueryHandler.Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(query))
                .Returns(Task.FromResult(conference));

            var result = await _controller.HealthAsync();
            var typedResult = (OkObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_internal_server_error_result_when_database_is_not_connected()
        {
            var exception = new AggregateException("database connection failed");
            
            _controller = new HealthCheckController(_mockQueryHandler.Object, _mockVideoPlatformService.Object);
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ThrowsAsync(exception);
            _mockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>()))
                .ReturnsAsync((TestCallResult)null);
            _mockVideoPlatformService
                .Setup(x => x.GetVirtualCourtRoomAsync(It.IsAny<Guid>()))
                .ReturnsAsync((MeetingRoom)null);

            var result = await _controller.HealthAsync();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            var response = (HealthCheckResponse) typedResult.Value;
            response.DatabaseHealth.Successful.Should().BeFalse();
            response.DatabaseHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }
        
        [Test]
        public async Task Should_return_internal_server_error_result_when_kinly_api_self_test_is_not_reachable()
        {
            var exception = new AggregateException("kinly self test api error");

            _controller = new HealthCheckController(_mockQueryHandler.Object, _mockVideoPlatformService.Object);
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(Builder<VideoApi.Domain.Conference>.CreateNew().Build);
            _mockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>()))
                .ThrowsAsync(exception);
            _mockVideoPlatformService
                .Setup(x => x.GetVirtualCourtRoomAsync(It.IsAny<Guid>()))
                .ReturnsAsync((MeetingRoom)null);

            var result = await _controller.HealthAsync();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            var response = (HealthCheckResponse) typedResult.Value;
            response.KinlySelfTestHealth.Successful.Should().BeFalse();
            response.KinlySelfTestHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }
        
        [Test]
        public async Task Should_return_internal_server_error_result_when_kinly_api_is_not_reachable()
        {
            var exception = new AggregateException("kinly api error");

            _controller = new HealthCheckController(_mockQueryHandler.Object, _mockVideoPlatformService.Object);
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(Builder<VideoApi.Domain.Conference>.CreateNew().Build);
            _mockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>()))
                .ReturnsAsync((TestCallResult)null);
            _mockVideoPlatformService
                .Setup(x => x.GetVirtualCourtRoomAsync(It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            var result = await _controller.HealthAsync();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            var response = (HealthCheckResponse) typedResult.Value;
            response.KinlyApiHealth.Successful.Should().BeFalse();
            response.KinlyApiHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task Should_return_the_application_version_from_assembly()
        {
            var result = await _controller.HealthAsync();
            var typedResult = (ObjectResult)result;
            var response = (HealthCheckResponse)typedResult.Value;
            response.AppVersion.FileVersion.Should().NotBeNullOrEmpty();
            response.AppVersion.InformationVersion.Should().NotBeNullOrEmpty();
        }
    }
}
