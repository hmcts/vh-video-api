using System;

namespace VideoApi.Contract.Responses
{
    public class InstantMessageResponse
    {
        public string From { get; set; }
        public string MessageText { get; set; }
        public DateTime TimeStamp { get; set; }
        public string To { get; set; }
    }
}
