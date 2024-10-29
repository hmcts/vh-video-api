namespace VideoApi.Common.Configuration
{
    public class WowzaConfiguration : IBlobStorageConfiguration
    {
        public string StreamingEndpoint { get; set; }
        public string ApplicationName { get; set; }
        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }
        public string StorageContainerName { get; set; }
        public string StorageEndpoint { get; set; }
        public string ManagedIdentityClientId { get; set; }
    }
}
