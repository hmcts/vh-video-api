using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;
using VideoApi.Services.Responses;
using HealthCheckResponse = VideoApi.Contract.Responses.HealthCheckResponse;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.HealthChecks
{
    public class HealthTests
    {
        private HealthCheckController _controller;
        private Mock<IQueryHandler> _mockQueryHandler;
        private Mock<IVideoPlatformService> _mockVideoPlatformService;
        private Mock<IAudioPlatformService> _mockAudioPlatformService;

        [SetUp]
        public void Setup()
        {
            _mockQueryHandler = new Mock<IQueryHandler>();
            _mockVideoPlatformService = new Mock<IVideoPlatformService>();
            _mockAudioPlatformService = new Mock<IAudioPlatformService>();
            
            // set all positive
            var conference = new ConferenceBuilder().Build();
            _mockQueryHandler
                .Setup(x =>
                    x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .Returns(Task.FromResult(conference));

            _mockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync((TestCallResult) null);
            
            _mockVideoPlatformService
                .Setup(x => x.GetPlatformHealthAsync())
                .ReturnsAsync(new VideoApi.Services.Kinly.HealthCheckResponse{Health_status = PlatformHealth.HEALTHY});
            
            var wowzaResponse = new []
            {
                new WowzaGetDiagnosticsResponse {ServerVersion = "1.0.0.1"},
                new WowzaGetDiagnosticsResponse {ServerVersion = "1.0.0.2"}
            };
            _mockAudioPlatformService.Setup(x => x.GetDiagnosticsAsync())
                .ReturnsAsync(wowzaResponse);
            
            _controller = new HealthCheckController(_mockQueryHandler.Object, _mockVideoPlatformService.Object,
                _mockAudioPlatformService.Object);
        }

        [Test]
        public async Task should_return_ok_when_all_dependencies_are_healthy()
        {
            var result = await _controller.HealthAsync();
            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_internal_server_error_result_when_database_is_not_connected()
        {
            var exception = new AggregateException("database connection failed");
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ThrowsAsync(exception);

            var result = await _controller.HealthAsync();
            var typedResult = (ObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            var response = (HealthCheckResponse)typedResult.Value;
            response.DatabaseHealth.Successful.Should().BeFalse();
            response.DatabaseHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

       // [Test]
        public async Task Should_return_internal_server_error_result_when_kinly_api_self_test_is_not_reachable()
        {
            var exception = new AggregateException("kinly self test api error");
            _mockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ThrowsAsync(exception);

            var result = await _controller.HealthAsync();
            var typedResult = (ObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            var response = (HealthCheckResponse)typedResult.Value;
            response.KinlySelfTestHealth.Successful.Should().BeFalse();
            response.KinlySelfTestHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task Should_return_internal_server_error_result_when_kinly_api_is_not_reachable()
        {
            var exception = new AggregateException("kinly api error");
            _mockVideoPlatformService
               .Setup(x => x.GetPlatformHealthAsync())
               .Throws(exception);

            var result = await _controller.HealthAsync();
            var typedResult = (ObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            var response = (HealthCheckResponse)typedResult.Value;
            response.KinlyApiHealth.Successful.Should().BeFalse();
            response.KinlyApiHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        //[Test]
        public async Task Should_return_internal_server_error_result_when_wowza_api_is_not_reachable()
        {
            var exception = new AggregateException("wowza api error");
            _mockAudioPlatformService
                .Setup(x => x.GetDiagnosticsAsync())
                .ThrowsAsync(exception);

            var result = await _controller.HealthAsync();
            var typedResult = (ObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            var response = (HealthCheckResponse)typedResult.Value;
            response.WowzaHealth.Successful.Should().BeFalse();
            response.WowzaHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
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
