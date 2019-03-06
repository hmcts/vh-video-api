using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using Testing.Common.Helper.Builders;
using Video.API.Controllers;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.UnitTests.Controllers
{
    public class HealthCheckControllerTests
    {
        protected Mock<IQueryHandler> MockRepository;
        protected HealthCheckController Controller { get; private set; }
        [SetUp]
        public void Setup()
        {
            MockRepository = new Mock<IQueryHandler>();
        }

        [Test]
        public async Task Should_return_ok_result_when_database_is_connectedAsync()
        {
            var hearingId = Guid.NewGuid();
            var conference = new ConferenceBuilder().Build();
            var query = new GetConferenceByIdQuery(hearingId);

            Controller = new HealthCheckController(MockRepository.Object);
            MockRepository.Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(query)).Returns(Task.FromResult(conference));

            var result = await Controller.Health();
            var typedResult = (OkResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_internal_server_error_result_when_database_is_not_connectedAsync()
        {
            var hearingId = Guid.NewGuid();
            var query = new GetConferenceByIdQuery(hearingId);
            Controller = new HealthCheckController(null);
            MockRepository.Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(query)).Throws<Exception>();

            var result = await Controller.Health();
            var typedResult = (ObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
