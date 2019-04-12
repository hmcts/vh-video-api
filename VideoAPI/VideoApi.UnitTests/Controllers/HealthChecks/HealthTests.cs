using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.UnitTests.Controllers.HealthChecks
{
    public class HealthTests
    {
        private Mock<IQueryHandler> _mockQueryHandler;
        private HealthCheckController _controller;
        
        [SetUp]
        public void Setup()
        {
            _mockQueryHandler = new Mock<IQueryHandler>();
        }

        [Test]
        public async Task Should_return_ok_result_when_database_is_connectedAsync()
        {
            var hearingId = Guid.NewGuid();
            var conference = new ConferenceBuilder().Build();
            var query = new GetConferenceByIdQuery(hearingId);

            _controller = new HealthCheckController(_mockQueryHandler.Object);
            _mockQueryHandler.Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(query)).Returns(Task.FromResult(conference));

            var result = await _controller.Health();
            var typedResult = (OkResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_internal_server_error_result_when_database_is_not_connectedAsync()
        {
            var hearingId = Guid.NewGuid();
            var query = new GetConferenceByIdQuery(hearingId);
            _controller = new HealthCheckController(null);
            _mockQueryHandler.Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(query)).Throws<Exception>();

            var result = await _controller.Health();
            var typedResult = (ObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
