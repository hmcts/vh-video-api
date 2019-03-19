using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Mappings;
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
                .Build();

            var response = _mapper.MapConferenceToResponse(conference);
            response.Should().BeEquivalentTo(conference, options => options
                .Excluding(x => x.HearingRefId)
                .Excluding(x => x.Participants)
            );

            response.CurrentStatus.Should().BeEquivalentTo(conference.GetCurrentStatus(), options => options
                .Excluding(x => x.Id)
            );
            
            var participants = conference.GetParticipants();
            response.Participants.Should().BeEquivalentTo(participants, options => options
                .Excluding(x => x.ParticipantRefId)
            );
        }
    }
}