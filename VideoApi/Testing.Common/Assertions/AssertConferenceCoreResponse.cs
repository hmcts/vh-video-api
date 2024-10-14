using System;
using FluentAssertions;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;

namespace Testing.Common.Assertions
{
    public class AssertConferenceCoreResponse
    {
        protected AssertConferenceCoreResponse()
        {
        }
        
        public static void ForConference(ConferenceCoreResponse conference, 
            ConferenceRoomType conferenceRoomType = ConferenceRoomType.VMR)
        {
            conference.Should().NotBeNull();
            conference.ScheduledDuration.Should().BeGreaterThan(0);
            conference.ScheduledDateTime.Should().NotBe(DateTime.MinValue);
            conference.CurrentStatus.Should().Be(conference.CurrentStatus);
            
            conference.IsWaitingRoomOpen.Should().BeTrue();
            
            
            if (conference.CurrentStatus > ConferenceState.NotStarted)
            {
                conference.StartedDateTime.Should().HaveValue().And.NotBe(DateTime.MinValue);
            }
            
            if (conference.CurrentStatus == ConferenceState.Closed)
            {
                conference.ClosedDateTime.Should().HaveValue().And.NotBe(DateTime.MinValue);
            }

            conference.ConferenceRoomType.Should().Be(conferenceRoomType);
        }
    }
}
