namespace Testing.Common.Configuration
{
    public class ZAPConfiguration
    {
        public string ApiAddress { get; set; }
        public int ApiPort { get; set; }
        public string Apikey { get; set; }
        public string WorkingDirectory { get; set; }
        public int MinutesToWait { get; set; }
        public bool RunZap { get; set; }
        public bool RunSpider { get; set; }
        public bool RunAjaxSpider { get; set; }
        public bool RunActiveScan { get; set; }
    }
}
