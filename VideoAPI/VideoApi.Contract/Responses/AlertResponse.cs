using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Responses
{
    public class AlertResponse
    {
        public long Id { get; set; }
        public string Body { get; set; }
        public AlertType Type { get; set; }
    }
}