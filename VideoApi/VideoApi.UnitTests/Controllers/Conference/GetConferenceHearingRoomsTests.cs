using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using VideoApi.Contract.Responses;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class GetConferenceHearingRoomsTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_return_ok_result_with_hearing_rooms_response()
        {
            var result = (OkObjectResult)await Controller.GetConferencesHearingRoomsAsync(DateTime.UtcNow.Date.ToString("yyyy-MM-dd"));
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().NotBeNull();
            var results = result.Value as IEnumerable<ConferenceHearingRoomsResponse>;
            results.Should().NotBeNull();
            results.Count().Should().Be(2);
        }

    }
}
