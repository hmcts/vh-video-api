namespace VideoApi.Services.Responses
{
    public class WowzaMonitorStreamResponse
    {
        public string ApplicationInstance { get; set; }
        public string Name { get; set; }
        public string ServerName { get; set; }
        public int Uptime { get; set; }
        public int BytesIn { get; set; }
        public int BytesInRate { get; set; }
    }
}
