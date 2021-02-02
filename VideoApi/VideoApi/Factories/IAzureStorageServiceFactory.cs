using VideoApi.Services.Contracts;

namespace Video.API.Factories
{
    public interface IAzureStorageServiceFactory
    {
        IAzureStorageService Create(AzureStorageServiceType azureStorageServiceType);
    }
}
