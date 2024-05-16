using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain;

public class Endpoint : TrackableEntity<Guid>
{
    public string DisplayName { get; private set; }
    public string SipAddress { get; }
    public string Pin { get; }
    public EndpointState State { get; private set; }
    
    [Obsolete("This property is only used for EF. Use EndpointParticipants instead")]
    public string DefenceAdvocate { get; }
    public RoomType? CurrentRoom { get; private set; }
    public long? CurrentConsultationRoomId { get; set; }
    public virtual ConsultationRoom CurrentConsultationRoom { get; set; }
    public virtual IList<EndpointParticipant> EndpointParticipants { get; }

    private Endpoint()
    {
        Id = Guid.NewGuid();
        State = EndpointState.NotYetJoined;
    }
    
    public Endpoint(string displayName, string sipAddress, string pin, string username) : this()
    {
        DisplayName = displayName;
        SipAddress = sipAddress;
        Pin = pin;
        EndpointParticipants = new List<EndpointParticipant>();
        AssignDefenceAdvocate(username);
    }

    public Endpoint(string displayName, string sipAddress, string pin, params (string username, LinkedParticipantType type)[] participants) : this()
    {
        DisplayName = displayName;
        SipAddress = sipAddress;
        Pin = pin;
        EndpointParticipants = new List<EndpointParticipant>();
        if (participants.Any())
            LinkParticipantsToEndpoint(participants);
    }

    public void UpdateDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    public void UpdateStatus(EndpointState status)
    {
        State = status;
    }
        
    public string GetCurrentRoom()
    {
        return CurrentConsultationRoom?.Label ?? CurrentRoom?.ToString() ?? throw new DomainRuleException(nameof(CurrentRoom), "Endpoint is not in a room");
    }

    public void UpdateCurrentRoom(RoomType? currentRoom)
    {
        CurrentRoom = currentRoom;
    }

    public void UpdateCurrentVirtualRoom(ConsultationRoom consultationRoom)
    {
        CurrentConsultationRoom?.RemoveEndpoint(new RoomEndpoint(Id));
        CurrentConsultationRoom = consultationRoom;
    }
          
    public void AssignDefenceAdvocate(string defenceAdvocate)
    {
        if(EndpointParticipants.Any(x => x.Type == LinkedParticipantType.DefenceAdvocate))
            EndpointParticipants.Remove(EndpointParticipants.First(x => x.Type == LinkedParticipantType.DefenceAdvocate));

        EndpointParticipants.Add(
            new EndpointParticipant(this, defenceAdvocate, LinkedParticipantType.DefenceAdvocate));
    }
        
    public string GetDefenceAdvocate()
    {
        return EndpointParticipants.FirstOrDefault(x => x.Type == LinkedParticipantType.DefenceAdvocate)?.Participant;
    }
        
    public void AssignIntermediary(string intermediary)
    {
        if(EndpointParticipants.Any(x => x.Type == LinkedParticipantType.Intermediary))
            EndpointParticipants.Remove(EndpointParticipants.First(x => x.Type == LinkedParticipantType.Intermediary));

        EndpointParticipants.Add(
            new EndpointParticipant(this, intermediary, LinkedParticipantType.Intermediary));
    }
        
    public string GetIntermediary()
    {
        return EndpointParticipants.FirstOrDefault(x => x.Type == LinkedParticipantType.Intermediary)?.Participant;
    }
        
    public void AssignRepresentative(string rep)
    {
        if(EndpointParticipants.Any(x => x.Type == LinkedParticipantType.Representative))
            EndpointParticipants.Remove(EndpointParticipants.First(x => x.Type == LinkedParticipantType.Representative));

        EndpointParticipants.Add(
            new EndpointParticipant(this, rep, LinkedParticipantType.Representative));
    }
        
    public string GetRepresentative()
    {
        return EndpointParticipants.FirstOrDefault(x => x.Type == LinkedParticipantType.Representative)?.Participant;
    }

    public void RemoveLinkedParticipant(string participant)
    {
        var linkedParticipant = EndpointParticipants.FirstOrDefault(x => x.Participant == participant);
        if(linkedParticipant != null)
            EndpointParticipants.Remove(linkedParticipant);
    }
        
    public void LinkParticipantsToEndpoint((string Username, LinkedParticipantType Type)[] participants)
    {
        foreach (var participant in participants)
        {
            switch (participant.Type)
            {
                case LinkedParticipantType.DefenceAdvocate:
                    AssignDefenceAdvocate(participant.Username);
                    break;
                case LinkedParticipantType.Intermediary:
                    AssignIntermediary(participant.Username);
                    break;
                case LinkedParticipantType.Representative:
                    AssignRepresentative(participant.Username);
                    break;
                default:
                    throw new ArgumentException("Invalid participant type for linking participant to endpoint", participant.Type.ToString());
            }
        }
    }
}
