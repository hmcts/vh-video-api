using System;
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
    public class GetConferenceByHearingRefIdTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_return_ok_result_for_given_valid_hearing_ref_id()
        {
            var result = await Controller.GetConferenceByHearingRefId(Guid.NewGuid());
            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            QueryHandlerMock
             .Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, Conference>(It.IsAny<GetConferenceByHearingRefIdQuery>()))
             .ReturnsAsync((Conference) null);


            var result = await Controller.GetConferenceByHearingRefId(Guid.NewGuid());
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
