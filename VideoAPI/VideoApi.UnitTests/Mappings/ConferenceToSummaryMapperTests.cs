using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Mappings;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Mappings
{
    public class ConferenceToSummaryMapperTests
    {
        private readonly ConferenceToSummaryResponseMapper _mapper = new ConferenceToSummaryResponseMapper();

        [Test]
        public void should_map_all_properties()
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipant(UserRole.Judge, "Judge")
                .WithParticipants(3)
                .WithHearingTask("Test1")
                .WithParticipantTask("Test2")
                .WithJudgeTask("Test3")
                .Build();

            var response = _mapper.MapConferenceToSummaryResponse(conference);
            response.Should().BeEquivalentTo(conference, options => options
                .Excluding(x => x.HearingRefId)
                .Excluding(x => x.Participants)
                .Excluding(x => x.ConferenceStatuses)
                .Excluding(x => x.State)
                .Excluding(x => x.Tasks)
            );
            response.Status.Should().BeEquivalentTo(conference.GetCurrentStatus());
            response.PendingTasks.Should().Be(conference.GetTasks().Count(x => x.Status == TaskStatus.ToDo));
        }
    }
}