using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public class Message : Entity<long>
    {
        public Message(string from, string messageText)
        {
            From = from;
            MessageText = messageText;
            TimeStamp = DateTime.UtcNow;
        }
        public string From { get; }
        public string To { get; }
        public string MessageText { get; }
        public DateTime TimeStamp { get; }
    }
}
