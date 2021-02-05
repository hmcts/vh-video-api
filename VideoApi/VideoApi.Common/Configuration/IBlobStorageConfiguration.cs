namespace VideoApi.Common.Configuration
{
    public interface IBlobStorageConfiguration
    {
        string ManagedIdentityClientId { get; set; }
        string StorageAccountKey { get; set; }
        string StorageAccountName { get; set; }
        string StorageContainerName { get; set; }
        string StorageEndpoint { get; set; }
    }
}
