namespace Testing.Common.Configuration
{
    public class ZapConfiguration
    {
        public string ApiAddress { get; set; }
        public int ApiPort { get; set; }
        public string ApiKey { get; set; }
        public string WorkingDirectory { get; set; }
        public int MinutesToWait { get; set; }
        public bool RunZap { get; set; }
        public bool RunSpider { get; set; }
        public bool RunAjaxSpider { get; set; }
        public bool RunActiveScan { get; set; }
    }
}
