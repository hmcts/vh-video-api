using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class GetHostsInHearingsTodayTests : ParticipantsControllerTestBase
    {
        [Test]
        public async Task Should_return_ok_result_and_many_hosts_from_multiple_conferences()
        {
            var conferences = new List<VideoApi.Domain.Conference>();
            conferences.AddRange(Enumerable.Range(1, 5).Select(x => BuildDefaultConference(x % 2 == 0
                ? UserRole.Judge
                : UserRole.StaffMember)));
            
            MockQueryHandler
                .Setup(x =>
                    x.Handle<GetHostsInHearingsTodayQuery, List<VideoApi.Domain.Conference>>(
                        It.IsAny<GetHostsInHearingsTodayQuery>()))
                .ReturnsAsync(conferences);
            
            var result = (OkObjectResult)await Controller.GetHostsInHearingsTodayAsync();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().NotBeNull();
            var results = result.Value as IEnumerable<ParticipantInHearingResponse>;
            results.Should().NotBeNull();
            results.Count().Should().Be(5);
        }
        
        private static VideoApi.Domain.Conference BuildDefaultConference(UserRole role)
        {
            return new ConferenceBuilder()
                .WithParticipant(role, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .Build();
        }
    }
}
