namespace VideoApi.Services.Responses
{
    public class WowzaMonitorStreamResponse
    {
        public int TotalConnections { get; set; }
        public string ApplicationInstance { get; set; }
        public int BytesOutRate { get; set; }
        public string Name { get; set; }
        public string ServerName { get; set; }
        public string[] SaveFieldList { get; set; }
        public string Version { get; set; }
        public int Uptime { get; set; }
        public int BytesIn { get; set; }
        public int BytesOut { get; set; }
        public int BytesInRate { get; set; }
    }
}
