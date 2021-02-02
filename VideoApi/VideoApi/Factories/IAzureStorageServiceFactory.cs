using VideoApi.Services.Contracts;

namespace VideoApi.Factories
{
    public interface IAzureStorageServiceFactory
    {
        IAzureStorageService Create(AzureStorageServiceType azureStorageServiceType);
    }
}
