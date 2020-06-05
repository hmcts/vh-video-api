using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public class InstantMessage : Entity<long>
    {
        public InstantMessage(string from, string messageText, string to)
        {
            From = from;
            MessageText = messageText;
            TimeStamp = DateTime.UtcNow;
            To = to;
        }
        public string From { get; }
        public string To { get; }
        public string MessageText { get; }
        public DateTime TimeStamp { get; }
        public Guid ConferenceId { set; get; }
    }
}
