using System;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class LinkedParticipantToResponseMapperTests
    {
        [Test]
        public void should_map_all_properties()
        {
            var linkedParticipant =
                new LinkedParticipant(Guid.NewGuid(), Guid.NewGuid(), LinkedParticipantType.Interpreter);

            var response = LinkedParticipantToResponseMapper.MapLinkedParticipantsToResponse(linkedParticipant);
            
            response.LinkedId.Should().Be(linkedParticipant.LinkedId);
            response.Type.Should().Be((Contract.Enums.LinkedParticipantType)linkedParticipant.Type);
        }
    }
}
