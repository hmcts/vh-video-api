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
            );
            response.Status.Should().BeEquivalentTo(conference.GetCurrentStatus());
            response.PendingTasks.Should().Be(conference.GetTasks().Count(x => x.Status == TaskStatus.ToDo));
        }

        [Test]
        public void should_map_only_active_tasks_properties()
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipant(UserRole.Judge, "Judge")
                .WithParticipants(3)
                .WithTask("Disconnected", TaskType.Hearing)
                .WithTask("Task1", TaskType.Hearing)
                .WithParticipantTask("Test2")
                .WithJudgeTask("Test3")
                .Build();

            conference.Tasks[0].Status = TaskStatus.Done;

            var response = _mapper.MapConferenceToSummaryResponse(conference);
            response.Tasks.Count.Should().BeGreaterThan(0);
            response.Tasks.Any(x => x.Status == TaskStatus.Done).Should().Be(false);
        }

        [Test]
        public void should_map_if_no_tasks_assign_to_conference()
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipant(UserRole.Judge, "Judge")
                .WithParticipants(3)
                .Build();

            var response = _mapper.MapConferenceToSummaryResponse(conference);
            response.Tasks.Count.Should().Be(0);
        }
    }
}