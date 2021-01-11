using System;
using System.Collections.Generic;
using Faker;
using FizzWare.NBuilder;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace Testing.Common.Helper.Builders.Domain
{
    public class RoomBuilder
    {
        private readonly Room _room;
        private readonly BuilderSettings _builderSettings;

        public RoomBuilder(Guid conferenceId)
        {
            _builderSettings = new BuilderSettings();
            var label = "label";
            var courtRoomType = VirtualCourtRoomType.JudgeJOH;
            _room = new Room(conferenceId, label, courtRoomType);
        }

        public RoomBuilder WithParticipants(int numberOfParticipants)
        {
            var roomParticipants = new Builder(_builderSettings).CreateListOfSize<RoomParticipant>(numberOfParticipants)
                .All()
                .WithFactory(() =>
                    new RoomParticipant(1, Guid.NewGuid())).Build();
            foreach (var participant in roomParticipants)
            {
                _room.AddParticipant(participant);
            }
            return this;
        }

        public Room Build()
        {
            return _room;
        }
    }
}
