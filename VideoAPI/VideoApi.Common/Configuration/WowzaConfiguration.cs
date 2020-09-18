namespace VideoApi.Common.Configuration
{
    public class WowzaConfiguration
    {
        public  string[] RestApiEndpoints { get; set; }
        public string StreamingEndpoint { get; set; }
        public string ServerName { get; set; }
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string StorageDirectory { get; set; }
        public string AzureStorageDirectory { get; set; }
        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }
        public string StorageContainerName { get; set; }
        public string StorageEndpoint { get; set; }
        public string ManagedIdentityClientId { get; set; }
    }
}
