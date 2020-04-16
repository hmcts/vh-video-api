namespace VideoApi.Services.Responses
{
    public class WowzaGetApplicationsResponse
    {
        public string ServerName { get; set; }
        public string[] SaveFieldList { get; set; }
        public string Version { get; set; }
        public Application[] Applications { get; set; }
    }

    public class Application
    {
        public bool DrmEnabled { get; set; }
        public bool StreamTargetsEnabled { get; set; }
        public string AppType { get; set; }
        public bool TranscoderEnabled { get; set; }
        public bool DvrEnabled { get; set; }
        public string Id { get; set; }
        public string Href { get; set; }
    }
}
