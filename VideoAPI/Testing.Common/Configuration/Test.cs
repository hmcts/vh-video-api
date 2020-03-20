using System;
using System.Collections.Generic;
using VideoApi.Contract.Requests;
using VideoApi.Domain;

namespace Testing.Common.Configuration
{
    public class Test
    {
        public Guid ParticipantId { get; set; }
        public AddHeartbeatRequest HeartbeatData { get; set; }
        public List<Conference> ClosedConferences { get; set; }
        public List<Conference> ClosedConferencesWithMessages { get; set; }
        public Conference Conference { get; set; }
        public List<Conference> Conferences { get; set; }
        public AddInstantMessageRequest Message { get; set; }
        public List<Conference> TodaysConferences { get; set; }
        public UpdateTaskRequest UpdateTaskRequest { get; set; }
        public Conference YesterdayClosedConference { get; set; }
    }
}
