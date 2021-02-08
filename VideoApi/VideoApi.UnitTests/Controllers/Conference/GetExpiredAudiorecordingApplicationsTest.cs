using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class GetExpiredAudiorecordingApplicationsTest : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_get_list_of_expired_conferences_with_audiorecording()
        {

            var conferences = await Controller.GetExpiredAudiorecordingConferencesAsync();

            QueryHandlerMock
                .Verify(x => x.Handle<GetExpiredAudiorecordingConferencesQuery, List<VideoApi.Domain.Conference>>(It.IsAny<GetExpiredAudiorecordingConferencesQuery>()), Times.Once);

            var result = (OkObjectResult)conferences;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().NotBeNull();
            var results = result.Value as IEnumerable<ExpiredConferencesResponse>;
            results.Should().NotBeNull();
            results.Count().Should().Be(1);
        }
    }
}
