using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers
{
    public class GetClosedConferences : MessageControllerTestBase
    {
        [Test]
        public async Task Should_return_a_not_found_result_when_no_closed_conferences_exist()
        {
            queryHandler
             .Setup(x => x.Handle<GetClosedConferencesQuery, List<Conference>>(It.IsAny<GetClosedConferencesQuery>()))
             .ReturnsAsync((List<Conference>)null);

            var result = await messageController.GetClosedConferences();
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_an_OK_result_for_when_closed_conferences_exist()
        {
            queryHandler
             .Setup(x => x.Handle<GetClosedConferencesQuery, List<Conference>>(It.IsAny<GetClosedConferencesQuery>()))
             .ReturnsAsync(new List<Conference>());

            var result = await messageController.GetClosedConferences();
            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}
