using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings;

public class ConferenceCoreResponseMapperTests
{
    [Test]
    public void Should_map_all_properties()
    {
        var conference = new ConferenceBuilder()
            .WithConferenceStatus(ConferenceState.InSession)
            .WithConferenceStatus(ConferenceState.Paused)
            .WithConferenceStatus(ConferenceState.Closed)
            .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
            .WithParticipants(3)
            .WithMessages(5)
            .WithInterpreterRoom()
            .Build();

        var response = ConferenceCoreResponseMapper.Map(conference);
        response.Should().BeEquivalentTo(conference, options => options
            .Excluding(x => x.HearingRefId)
            .Excluding(x => x.Participants)
            .Excluding(x => x.ConferenceStatuses)
            .Excluding(x => x.State)
            .Excluding(x => x.InstantMessageHistory)
            .Excluding(x => x.IngestUrl)
            .Excluding(x => x.ActualStartTime)
            .Excluding(x => x.Endpoints)
            .Excluding(x => x.CreatedDateTime)
            .Excluding(x => x.Rooms)
            .Excluding(x => x.UpdatedAt)
            .Excluding(x => x.Supplier)
            .Excluding(x => x.CaseName)
            .Excluding(x => x.CaseNumber)
            .Excluding(x => x.CaseType)
            .Excluding(x => x.HearingVenueName)
            .Excluding(x => x.Supplier)
            .Excluding(x => x.AudioRecordingRequired)
            .Excluding(x => x.MeetingRoom)
            .Excluding(x => x.IngestUrlFilenamePrefix)
        );
        
        response.StartedDateTime.Should().HaveValue().And.Be(conference.ActualStartTime);
        response.ClosedDateTime.Should().HaveValue().And.Be(conference.ClosedDateTime);
        response.CurrentStatus.Should().Be((Contract.Enums.ConferenceState)conference.GetCurrentStatus());
        
        var participants = conference.GetParticipants();
        response.Participants.Should().BeEquivalentTo(participants, options => options
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
            .Excluding(x => x.UpdatedAt)
            .Excluding(x => x.CreatedAt)
            .Excluding(x => x.Name)
            .Excluding(x => x.HearingRole)
            .Excluding(x => x.Username)
        );
    }
}
