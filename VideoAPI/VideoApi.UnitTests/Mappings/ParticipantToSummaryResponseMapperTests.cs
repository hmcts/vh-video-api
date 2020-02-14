using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Mappings;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Mappings
{
    public class ParticipantToSummaryResponseMapperTests
    {
        private readonly ParticipantToSummaryResponseMapper _mapper = new ParticipantToSummaryResponseMapper();

        [Test]
        public void should_map_all_properties()
        {
            var participant = new ParticipantBuilder().Build();
            participant.UpdateParticipantStatus(ParticipantState.Available);
            var response = _mapper.MapParticipantToSummary(participant);
            response.Should().BeEquivalentTo(participant, options => options
                .Excluding(x => x.ParticipantRefId)
                .Excluding(x => x.DisplayName)
                .Excluding(x => x.Name)
                .Excluding(x => x.UserRole)
                .Excluding(x => x.Id)
                .Excluding(x => x.CaseTypeGroup)
                .Excluding(x => x.Representee)
                .Excluding(x => x.TestCallResultId)
                .Excluding(x => x.TestCallResult)
                .Excluding(x => x.CurrentRoom)
                .Excluding(x => x.State)
            );
            response.Status.Should().BeEquivalentTo(participant.State);
            response.Status.Should().BeEquivalentTo(participant.GetCurrentStatus().ParticipantState);
            if (participant.UserRole == UserRole.Individual || participant.UserRole == UserRole.Representative)
            {
                response.CaseGroup.Should().Be(participant.CaseTypeGroup);
            }
            else
            {
                response.CaseGroup.Should().BeEmpty();
            }
        }
    }
}
