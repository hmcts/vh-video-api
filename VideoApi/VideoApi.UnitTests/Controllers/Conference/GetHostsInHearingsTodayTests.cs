using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class GetHostsInHearingsTodayTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_return_ok_result_and_many_judges_only_from_multiple_conferences()
        {
            var conferences = new List<VideoApi.Domain.Conference>();
            conferences.AddRange(Enumerable.Range(1, 5).Select(x => BuildDefaultConference()));
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetHostsInHearingsTodayQuery, List<VideoApi.Domain.Conference>>(It.IsAny<GetHostsInHearingsTodayQuery>()))
                .ReturnsAsync(conferences);

            var result = (OkObjectResult)await Controller.GetJudgesInHearingsTodayAsync();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().NotBeNull();
            var results = result.Value as IEnumerable<ParticipantInHearingResponse>;
            results.Should().NotBeNull();
            results.Count().Should().Be(5);
        }

        private static VideoApi.Domain.Conference BuildDefaultConference()
        {
            return new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .Build();
        }
    }
}
