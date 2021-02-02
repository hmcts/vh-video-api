using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class ParticipantForJudgeResponseMapperTests
    {
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
        public void Should_map_all_participants(Participant participant)
        {
            var response = ParticipantForJudgeResponseMapper.MapParticipantSummaryToModel(participant);

            response.DisplayName.Should().BeEquivalentTo(participant.DisplayName);
            response.Role.Should().BeEquivalentTo(participant.UserRole);
            response.Representee.Should().BeEquivalentTo(participant.Representee);
            response.CaseTypeGroup.Should().BeEquivalentTo(participant.CaseTypeGroup);
            response.HearingRole.Should().BeEquivalentTo(participant.HearingRole);
        }
    }
}
