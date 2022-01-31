using VideoApi.Services.Contracts;

namespace VideoApi.Services.Factories
{
    public interface IAzureStorageServiceFactory
    {
        IAzureStorageService Create(AzureStorageServiceType azureStorageServiceType);
    }
}
