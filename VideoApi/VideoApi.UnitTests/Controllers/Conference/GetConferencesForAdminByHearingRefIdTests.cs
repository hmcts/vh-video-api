using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class GetConferencesForAdminByHearingRefIdTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_return_ok_result_for_given_valid_hearing_ref_id()
        {
            var result = await Controller.GetConferencesForAdminByHearingRefIdAsync(new []{Guid.NewGuid()});
            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            typedResult.Value.Should().BeOfType<List<ConferenceForAdminResponse>>();
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            QueryHandlerMock
             .Setup(x => x.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<VideoApi.Domain.Conference>>(It.IsAny<GetNonClosedConferenceByHearingRefIdQuery>()))
             .ReturnsAsync((List<VideoApi.Domain.Conference>) null);
            
            var result = await Controller.GetConferencesForAdminByHearingRefIdAsync(new []{Guid.NewGuid()});
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
