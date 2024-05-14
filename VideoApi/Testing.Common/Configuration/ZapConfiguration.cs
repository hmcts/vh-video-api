
namespace Testing.Common.Configuration
{
    public class ZapConfiguration
    {
        public string ApiAddress { get; set; }
        public int ApiPort { get; set; }
        public string ApiConfigPath { get; set; }
        public bool ZapScan { get; set; }
        public bool ActiveScan { get; set; }
        public bool SetUpProxy { get; set; }
        public string ServiceName { get; set; }
        public string SolutionFolderName { get; set; }
        public bool SkipPublish { get; set; }
        public bool HeadlessMode { get; set; }
        public string BuildConfigMode { get; set; }
    }
}
