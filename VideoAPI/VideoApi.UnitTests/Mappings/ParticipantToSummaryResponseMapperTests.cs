using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Mappings;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Mappings
{
    public class ParticipantToSummaryResponseMapperTests
    {
        private readonly ParticipantToSummaryResponseMapper _mapper = new ParticipantToSummaryResponseMapper();

        public static IEnumerable<Participant> ParticipantTestCases
        {
            get
            {
                yield return new ParticipantBuilder().WithUserRole(UserRole.Individual).Build();
                yield return new ParticipantBuilder().WithUserRole(UserRole.Representative).Build();
                yield return new ParticipantBuilder().WithUserRole(UserRole.CaseAdmin).Build();
            }
        }

        [TestCaseSource("ParticipantTestCases")]
        public void Should_map_all_properties(Participant participant)
        {
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
