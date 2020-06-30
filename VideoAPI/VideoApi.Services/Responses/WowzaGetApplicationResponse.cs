namespace VideoApi.Services.Responses
{
    public class WowzaGetApplicationResponse
    {
        public Streamconfig StreamConfig { get; set; }
        public string ServerName { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }

    public class Streamconfig
    {
        public string StreamType { get; set; }
        public bool StorageDirExists { get; set; }
        public string KeyDir { get; set; }
        public string ServerName { get; set; }
        public string StorageDir { get; set; }
    }
}
