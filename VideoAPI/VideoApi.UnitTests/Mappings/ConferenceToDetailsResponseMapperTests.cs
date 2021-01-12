using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Mappings;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Mappings
{
    public class ConferenceToDetailsResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .WithParticipants(3)
                .WithMessages(5)
                .Build();

            var pexipSelfTestNode = "selttest@pexip.node";
            var response = ConferenceToDetailsResponseMapper.MapConferenceToResponse(conference, pexipSelfTestNode);
            response.Should().BeEquivalentTo(conference, options => options
                .Excluding(x => x.HearingRefId)
                .Excluding(x => x.Participants)
                .Excluding(x => x.ConferenceStatuses)
                .Excluding(x => x.State)
                .Excluding(x => x.InstantMessageHistory)
                .Excluding(x => ExcludeIdFromMessage(x))
                .Excluding(x => x.IngestUrl)
                .Excluding(x => x.ActualStartTime)
                .Excluding(x => x.Endpoints)
             );

            response.StartedDateTime.Should().HaveValue().And.Be(conference.ActualStartTime);
            response.ClosedDateTime.Should().HaveValue().And.Be(conference.ClosedDateTime);
            response.CurrentStatus.Should().BeEquivalentTo(conference.GetCurrentStatus());

            var participants = conference.GetParticipants();
            response.Participants.Should().BeEquivalentTo(participants, options => options
                .Excluding(x => x.ParticipantRefId)
                .Excluding(x => x.TestCallResultId)
                .Excluding(x => x.TestCallResult)
                .Excluding(x => x.CurrentVirtualRoomId)
                .Excluding(x => x.CurrentVirtualRoom)
                .Excluding(x => x.CurrentRoom)
                .Excluding(x => x.State)
            );
        }

        
        bool ExcludeIdFromMessage(IMemberInfo member)
        {
            return member.SelectedMemberPath.Contains(nameof(InstantMessage)) && member.SelectedMemberInfo.Name.Contains("Id");
        }
    }
}
