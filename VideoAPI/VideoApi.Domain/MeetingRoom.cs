namespace VideoApi.Domain
{
    public class MeetingRoom
    {
        internal MeetingRoom()
        {
        }
        
        public MeetingRoom(string adminUri, string judgeUri, string participantUri, string pexipNode, string telephoneConferenceId)
        {
            AdminUri = adminUri;
            JudgeUri = judgeUri;
            ParticipantUri = participantUri;
            PexipNode = pexipNode;
            TelephoneConferenceId = telephoneConferenceId;
        }

        public string AdminUri { get; set; }
        public string JudgeUri { get; set; }
        public string ParticipantUri { get; set; }
        public string PexipNode { get; set; }
        public string TelephoneConferenceId { get; set; }

        public bool IsSet()
        {
            return AdminUri != null && JudgeUri != null && ParticipantUri != null && PexipNode != null && TelephoneConferenceId != null;
        }
    }
}
