using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
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
            results.Count().Should().Be(4);
        }

        [Test]
        public async Task Should_return_Exception_when_service_method_throws_exception()
        {
            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetConferenceInterpreterRoomsByDateQuery, List<VideoApi.Domain.HearingAudioRoom>>(
                        It.IsAny<GetConferenceInterpreterRoomsByDateQuery>()))
                .Throws(new Exception());

            var result = (NoContentResult)await Controller.GetConferencesHearingRoomsAsync(DateTime.UtcNow.Date.ToString("yyyy-MM-dd"));
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

        }

    }
}
