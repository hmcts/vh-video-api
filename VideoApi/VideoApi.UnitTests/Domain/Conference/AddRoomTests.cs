using System;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class AddRoomTests
    {
        [Test]
        public void Should_add_room_to_conference()
        {
            var conference = new ConferenceBuilder().Build();
            var room = new ConsultationRoom(conference.Id, "ParticipantConsultationRoom1", VirtualCourtRoomType.Participant, false);
            conference.AddRoom(room);
            conference.Rooms.Should().Contain(x=> x.Label == "ParticipantConsultationRoom1");
        }
        
        [Test]
        public void Should_add_room_to_conference_with_no_label()
        {
            var conference = new ConferenceBuilder().Build();
            var room = new ConsultationRoom(conference.Id, VirtualCourtRoomType.JudicialShared, false);
            conference.AddRoom(room);
            conference.Rooms.Should().Contain(x => x.Type == VirtualCourtRoomType.JudicialShared);
        }
        
        [Test]
        public void Should_throw_argument_null_exception_when_adding_a_null_room()
        {
            var conference = new ConferenceBuilder().Build();
            var action = () => conference.AddRoom(null);
            action.Should().Throw<ArgumentNullException>();
        }
        
        [Test]
        public void should_throw_domain_rule_exception_when_adding_a_room_that_is_already_added()
        {
            var conference = new ConferenceBuilder().Build();
            var room = new ConsultationRoom(conference.Id, "ParticipantConsultationRoom1",
                VirtualCourtRoomType.Participant, false);
            conference.AddRoom(room);
            var action = () => conference.AddRoom(room);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == $"Room {room.Label} already exists in conference and is still open").Should()
                .BeTrue();
        }
        
        [Test]
        public void should_not_throw_domain_rule_exception_when_adding_a_room_that_is_already_added_and_closed()
        {
            var conference = new ConferenceBuilder().Build();
            var room = new ConsultationRoom(conference.Id, "ParticipantConsultationRoom1", VirtualCourtRoomType.Participant, false);
            room.CloseRoom();
            var action = () => conference.AddRoom(room);
            action.Should().NotThrow<DomainRuleException>();
        }
    }
}
