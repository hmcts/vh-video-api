using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class GetAvailableConsultationRoomTests
    {
        [Test]
        public void Should_return_first_consultation_room_when_no_rooms_occupied()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant")
                .Build();

            var availableRoom = conference.GetAvailableConsultationRoom();
            availableRoom.Should().Be(RoomType.ConsultationRoom1);
        }
        
        [Test]
        public void Should_return_second_consultation_room_when_first_is_occupied()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant")
                .Build();
            
            conference.Participants[0].UpdateCurrentRoom(RoomType.ConsultationRoom1);

            var availableRoom = conference.GetAvailableConsultationRoom();
            availableRoom.Should().Be(RoomType.ConsultationRoom2);
        }
        
        [Test]
        public void Should_return_first_consultation_room_when_second_is_occupied()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant")
                .Build();
            
            conference.Participants[0].UpdateCurrentRoom(RoomType.ConsultationRoom2);

            var availableRoom = conference.GetAvailableConsultationRoom();
            availableRoom.Should().Be(RoomType.ConsultationRoom1);
        }
        
        [Test]
        public void Should_throw_exception_when_no_participants_in_conference()
        {
            var conference = new ConferenceBuilder()
                .Build();
            
            Action action = () => conference.GetAvailableConsultationRoom();
            action.Should().Throw<DomainRuleException>().And.ValidationFailures.Any(x => x.Name == "No Participants")
                .Should().BeTrue();
        }

        [Test]
        public void Should_throw_exception_when_no_room_is_available()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant")
                .Build();
            
            conference.Participants[0].UpdateCurrentRoom(RoomType.ConsultationRoom1);
            conference.Participants[1].UpdateCurrentRoom(RoomType.ConsultationRoom2);

            Action action = () => conference.GetAvailableConsultationRoom();
            action.Should().Throw<DomainRuleException>().And.ValidationFailures.Any(x => x.Name == "Unavailable room")
                .Should().BeTrue();
        }
    }
}