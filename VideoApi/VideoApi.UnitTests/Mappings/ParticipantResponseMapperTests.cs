using System;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class ParticipantResponseMapperTests
    {
        [Test]
        public void should_map_participant_to_response_without_interpreter_room()
        {
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual).Build();

            var response = ParticipantResponseMapper.Map(participant);

            response.Should().BeEquivalentTo(participant, options => options
                .Excluding(x => x.ParticipantRefId)
                .Excluding(x => x.ConferenceId)
                .Excluding(x => x.TestCallResultId)
                .Excluding(x => x.TestCallResult)
                .Excluding(x => x.CurrentConsultationRoomId)
                .Excluding(x => x.CurrentConsultationRoom)
                .Excluding(x => x.CurrentRoom)
                .Excluding(x => x.State)
                .Excluding(x => x.LinkedParticipants)
                .Excluding(x => x.RoomParticipants)
                .Excluding(x => x.CreatedAt)
                .Excluding(x => x.UpdatedAt)
                .Excluding(x => x.FirstName)
                .Excluding(x => x.LastName)
                .Excluding(x => x.ContactTelephone)
                .Excluding(x => x.CaseTypeGroup)
                .Excluding(x => x.Representee)
                .Excluding(x => x.HearingRole)
                .Excluding(x => x.Name));
        }

        [Test]
        public void should_map_participant_to_response_wit_interpreter_room()
        {
            var consultationRoom = new ConsultationRoom(Guid.NewGuid(), "ParticipantRoom1",
                VirtualCourtRoomType.Participant, true);
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual).Build();
            participant.UpdateCurrentConsultationRoom(consultationRoom);
            var interpreterRoom = new ParticipantRoom(Guid.NewGuid(), "Interpreter1", VirtualCourtRoomType.Witness);
            
            var response = ParticipantResponseMapper.Map(participant, interpreterRoom);
            
            response.Should().BeEquivalentTo(participant, options => options
                .Excluding(x => x.ParticipantRefId)
                .Excluding(x => x.ConferenceId)
                .Excluding(x => x.TestCallResultId)
                .Excluding(x => x.TestCallResult)
                .Excluding(x => x.CurrentConsultationRoomId)
                .Excluding(x => x.CurrentConsultationRoom)
                .Excluding(x => x.CurrentRoom)
                .Excluding(x => x.State)
                .Excluding(x => x.LinkedParticipants)
                .Excluding(x => x.RoomParticipants)
                .Excluding(x => x.CreatedAt)
                .Excluding(x => x.UpdatedAt)
                .Excluding(x => x.FirstName)
                .Excluding(x => x.LastName)
                .Excluding(x => x.ContactTelephone)
                .Excluding(x => x.CaseTypeGroup)
                .Excluding(x => x.Representee)
                .Excluding(x => x.HearingRole)
                .Excluding(x => x.Name));

            response.CurrentInterpreterRoom.Should().NotBeNull();
            response.CurrentInterpreterRoom.Id.Should().Be(interpreterRoom.Id);
            response.CurrentInterpreterRoom.Label.Should().Be(interpreterRoom.Label);
            response.CurrentInterpreterRoom.Locked.Should().BeFalse();

            response.CurrentRoom.Should().NotBeNull();
            response.CurrentRoom.Id.Should().Be(consultationRoom.Id);
            response.CurrentRoom.Label.Should().Be(consultationRoom.Label);
            response.CurrentRoom.Locked.Should().Be(consultationRoom.Locked);
        }
    }
}
