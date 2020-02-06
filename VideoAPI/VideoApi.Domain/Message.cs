using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public class Message : Entity<long>
    {
        public Message(string from, string to, string messageText)
        {
            From = from;
            To = to;
            MessageText = messageText;
            TimeStamp = DateTime.UtcNow;
        }
        public string From { get; set; }
        public string To { get; set; }
        public string MessageText { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}