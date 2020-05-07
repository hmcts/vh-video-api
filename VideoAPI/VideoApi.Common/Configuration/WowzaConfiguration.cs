namespace VideoApi.Common.Configuration
{
    public class WowzaConfiguration
    {
        public string RestApiEndpoint { get; set; }
        public string StreamingEndpoint { get; set; }
        public string ServerName { get; set; }
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string StorageDirectory { get; set; }
        public string AzureStorageDirectory { get; set; }
    }
}
