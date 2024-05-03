using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class GetConferenceDetailsByIdTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_return_ok_result_for_given_conference_id()
        {
            var result = await Controller.GetConferenceDetailsByIdAsync(TestConference.Id);

            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync((VideoApi.Domain.Conference)null);             


            var result = await Controller.GetConferenceDetailsByIdAsync(Guid.NewGuid());
            var typedResult = (NotFoundObjectResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
