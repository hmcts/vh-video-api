namespace VideoApi.Common.Configuration
{
    public class CvpConfiguration : IBlobStorageConfiguration
    {
        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }
        public string StorageContainerName { get; set; }
        public string StorageEndpoint { get; set; }
        public string ManagedIdentityClientId { get; set; }
    }
}
