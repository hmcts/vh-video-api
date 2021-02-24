using FluentAssertions;
using NUnit.Framework;
using System;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain
{
    public class RoomTest
    {
        [Test]
        public void Should_create_room_and_set_status_to_created()
        {
            var conferenceId = Guid.NewGuid();
            var label = "Interpreter1";

            var room = new Room(conferenceId, label, VirtualCourtRoomType.JudgeJOH, false);
            room.Status.Should().Be(RoomStatus.Live);
            room.ConferenceId.Should().Be(conferenceId);
            room.Label.Should().Be(label);
            room.Type.Should().Be(VirtualCourtRoomType.JudgeJOH);
        }

        [Test]
        public void Should_add_participant_to_room()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH, false);
            room.AddParticipant(roomParticipant);

            room.RoomParticipants.Count.Should().Be(1);
            room.RoomParticipants[0].ParticipantId.Should().Be(participantId);
        }

        [Test]
        public void Should_add_endpoint_to_room()
        {
            var endpointId = Guid.NewGuid();
            var roomEndpoint = new RoomEndpoint(endpointId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.Participant, false);
            room.AddEndpoint(roomEndpoint);

            room.RoomEndpoints.Count.Should().Be(1);
            room.RoomEndpoints[0].EndpointId.Should().Be(endpointId);
        }

        [Test]
        public void Should_not_add_existing_participant_to_room_and_throw_exception()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH, false);
            room.AddParticipant(roomParticipant);
            var beforeCount = room.RoomParticipants.Count;

            Action action = () => room.AddParticipant(roomParticipant);

            action.Should().Throw<DomainRuleException>();
            room.RoomParticipants.Count.Should().Be(beforeCount);
        }

        [Test]
        public void Should_not_add_existing_endpoint_to_room_and_throw_exception()
        {
            var endpointId = Guid.NewGuid();
            var roomEndpoint = new RoomEndpoint(endpointId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.Participant, false);
            room.AddEndpoint(roomEndpoint);
            var beforeCount = room.RoomEndpoints.Count;

            Action action = () => room.AddEndpoint(roomEndpoint);

            action.Should().Throw<DomainRuleException>();
            room.RoomEndpoints.Count.Should().Be(beforeCount);
        }

        [Test]
        public void Should_remove_participant_from_room()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH, false);
            room.AddParticipant(roomParticipant);
            var beforeCount = room.RoomParticipants.Count;

            room.RemoveParticipant(roomParticipant);

            room.RoomParticipants.Count.Should().Be(beforeCount - 1);
        }

        [Test]
        public void Should_remove_endpoint_from_room()
        {
            var endpointId = Guid.NewGuid();
            var roomEndpoint = new RoomEndpoint(endpointId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.Participant, false);
            room.AddEndpoint(roomEndpoint);
            var beforeCount = room.RoomEndpoints.Count;

            room.RemoveEndpoint(roomEndpoint);

            room.RoomEndpoints.Count.Should().Be(beforeCount - 1);
        }

        [Test]
        public void Should_throw_exception_for_remove_non_existing_participant_from_room()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH, false);
            room.AddParticipant(roomParticipant);
            var beforeCount = room.RoomParticipants.Count;

           Action action = () => room.RemoveParticipant(new RoomParticipant(Guid.NewGuid()));

            action.Should().Throw<DomainRuleException>();
            room.RoomParticipants.Count.Should().Be(beforeCount);
        }


        [Test]
        public void Should_throw_exception_for_remove_non_existing_endpoint_from_room()
        {
            var endpointId = Guid.NewGuid();
            var roomEndpoint = new RoomEndpoint(endpointId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.Participant, false);
            room.AddEndpoint(roomEndpoint);
            var beforeCount = room.RoomEndpoints.Count;

            Action action = () => room.RemoveEndpoint(new RoomEndpoint(Guid.NewGuid()));

            action.Should().Throw<DomainRuleException>();
            room.RoomEndpoints.Count.Should().Be(beforeCount);
        }

        [Test]
        public void Should_update_room_status_to_Live_on_init()
        {
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH, false);
            room.Status.Should().Be(RoomStatus.Live);
        }

        [Test]
        public void Should_update_room_status_to_closed_on_last_participant_remove()
        {
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH, false);
            var roomParticipant = new RoomParticipant(Guid.NewGuid());
            room.RoomParticipants.Add(roomParticipant);
            room.Status.Should().Be(RoomStatus.Live);

            room.RemoveParticipant(roomParticipant);

            room.Status.Should().Be(RoomStatus.Closed);
        }

        [Test]
        public void Should_update_room_status_to_closed_on_last_endpoint_remove()
        {
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.Participant, false);
            var roomEndpoint = new RoomEndpoint(Guid.NewGuid());
            room.RoomEndpoints.Add(roomEndpoint);
            room.Status.Should().Be(RoomStatus.Live);

            room.RemoveEndpoint(roomEndpoint);

            room.Status.Should().Be(RoomStatus.Closed);
        }

        [Test]
        public void Should_not_update_room_status_to_closed_on_last_endpoint_remove_if_has_participant()
        {
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.Participant, false);
            var roomParticipant = new RoomParticipant(Guid.NewGuid());
            room.RoomParticipants.Add(roomParticipant);
            var roomEndpoint = new RoomEndpoint(Guid.NewGuid());
            room.RoomEndpoints.Add(roomEndpoint);
            room.Status.Should().Be(RoomStatus.Live);

            room.RemoveEndpoint(roomEndpoint);

            room.Status.Should().Be(RoomStatus.Live);
        }


        [Test]
        public void Should_not_update_room_status_to_closed_on_last_participant_remove_if_has_endpoint()
        {
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.Participant, false);
            var roomParticipant = new RoomParticipant(Guid.NewGuid());
            room.RoomParticipants.Add(roomParticipant);
            var roomEndpoint = new RoomEndpoint(Guid.NewGuid());
            room.RoomEndpoints.Add(roomEndpoint);
            room.Status.Should().Be(RoomStatus.Live);

            room.RemoveParticipant(roomParticipant);

            room.Status.Should().Be(RoomStatus.Live);
        }

        [Test]
        public void Should_return_list_of_participant_in_a_room()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH, false);
            room.AddParticipant(roomParticipant);

            room.RoomParticipants.Count.Should().Be(1);
            room.RoomParticipants[0].ParticipantId.Should().Be(participantId);

            var participantsList = room.GetRoomParticipants();
            participantsList.Count.Should().Be(1);
        }

        [Test]
        public void should_add_room_connection_details()
        {
            var room = new Room(Guid.NewGuid(), VirtualCourtRoomType.Civilian, false);
            room.Label.Should().BeNull();

            var label = "Interpreter1";
            var ingestUrl = $"rtmps://hostserver/hearingId1/hearingId1/{room.Id}";
            var node = "sip.test.com";
            var participantUri = "env-foo-interpeterroom";
            
            room.AddRoomConnectionDetails(label, ingestUrl, node, participantUri);

            room.Label.Should().Be(label);
            room.IngestUrl.Should().Be(ingestUrl);
            room.PexipNode.Should().Be(node);
            room.ParticipantUri.Should().Be(participantUri);
        }
    }
}
