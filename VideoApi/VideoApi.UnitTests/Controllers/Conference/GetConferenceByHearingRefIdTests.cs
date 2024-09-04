using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class GetConferenceByHearingRefIdTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_return_ok_result_for_given_valid_hearing_ref_id()
        {
            var request = new GetConferencesByHearingIdsRequest { HearingRefIds = [TestConference.HearingRefId] };
            var result = await Controller.GetConferencesByHearingRefIdsAsync(request);
            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        
        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<VideoApi.Domain.Conference>>(
                        It.IsAny<GetNonClosedConferenceByHearingRefIdQuery>()))
                .ReturnsAsync(new List<VideoApi.Domain.Conference>());
            
            var result = await Controller.GetConferencesByHearingRefIdsAsync(new GetConferencesByHearingIdsRequest
                { HearingRefIds = [TestConference.HearingRefId] });
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
