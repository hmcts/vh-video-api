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
            );
            response.Status.Should().BeEquivalentTo(participant.GetCurrentStatus().ParticipantState);
        }
    }
}