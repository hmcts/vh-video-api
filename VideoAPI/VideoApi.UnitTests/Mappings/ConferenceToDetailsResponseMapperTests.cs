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
        private readonly ConferenceToDetailsResponseMapper _mapper = new ConferenceToDetailsResponseMapper();

        [Test]
        public void should_map_all_properties()
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipants(3)
                .WithMessages(5)
                .Build();

            var pexipSelfTestNode = "selttest@pexip.node";
            var response = _mapper.MapConferenceToResponse(conference, pexipSelfTestNode);
            response.Should().BeEquivalentTo(conference, options => options
                .Excluding(x => x.HearingRefId)
                .Excluding(x => x.Participants)
                .Excluding(x => x.ConferenceStatuses)
                .Excluding(x => x.State)
                .Excluding(x => x.Tasks)
                .Excluding(x => ExcludeIdFromMessage(x))
             );

            response.CurrentStatus.Should().BeEquivalentTo(conference.GetCurrentStatus());

            var participants = conference.GetParticipants();
            response.Participants.Should().BeEquivalentTo(participants, options => options
                .Excluding(x => x.ParticipantRefId)
                .Excluding(x => x.TestCallResultId)
                .Excluding(x => x.TestCallResult)
                .Excluding(x => x.CurrentRoom)
            );
        }

        
        bool ExcludeIdFromMessage(IMemberInfo member)
        {
            return member.SelectedMemberPath.Contains(nameof(Message)) && member.SelectedMemberInfo.Name.Contains("Id");
        }
    }
}
