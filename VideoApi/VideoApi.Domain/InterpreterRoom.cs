using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class InterpreterRoom : Room
    {
        public string IngestUrl { get; private set; }
        public string PexipNode { get; private set; }
        public string ParticipantUri { get; private set; }

        public InterpreterRoom(Guid conferenceId, string label, VirtualCourtRoomType type) : base(conferenceId, label,
            type, false)
        {
        }

        public InterpreterRoom(Guid conferenceId, VirtualCourtRoomType type) : base(conferenceId, type, false)
        {
        }

        public void UpdateConnectionDetails(string label, string ingestUrl, string pexipNode, string participantUri)
        {
            Label = label;
            IngestUrl = ingestUrl;
            PexipNode = pexipNode;
            ParticipantUri = participantUri;
        }
    }
}
