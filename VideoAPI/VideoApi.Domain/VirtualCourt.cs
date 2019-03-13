using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public class VirtualCourt : Entity<long>
    {
        public VirtualCourt(string adminUri, string judgeUri, string participantUri, string pexipNode)
        {
            AdminUri = adminUri;
            JudgeUri = judgeUri;
            ParticipantUri = participantUri;
            PexipNode = pexipNode;
        }

        public string AdminUri { get; set; }
        public string JudgeUri { get; set; }
        public string ParticipantUri { get; set; }
        public string PexipNode { get; set; }
    }
}