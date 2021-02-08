using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class ParticipantToSummaryResponseMapperTests
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

        [TestCaseSource(nameof(ParticipantTestCases))]
        public void Should_map_all_properties(Participant participant)
        {
            participant.UpdateParticipantStatus(ParticipantState.Available);
            var response = ParticipantToSummaryResponseMapper.MapParticipantToSummary(participant);
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
                .Excluding(x => x.CurrentVirtualRoomId)
                .Excluding(x => x.CurrentVirtualRoom)
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