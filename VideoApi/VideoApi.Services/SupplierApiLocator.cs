using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Services.Clients;

namespace VideoApi.Services;

public interface ISupplierApiSelector
{
    public ISupplierApiClient GetHttpClient();
    public SupplierConfiguration GetSupplierConfiguration();
}

public class SupplierApiSelector(
    IServiceProvider serviceProvider,
    IFeatureToggles featureToggles,
    IOptions<KinlyConfiguration> kinlyConfigOptions,
    IOptions<VodafoneConfiguration> vodafoneConfigOptions)
    : ISupplierApiSelector
{
    public ISupplierApiClient GetHttpClient() => featureToggles.VodafoneIntegrationEnabled()
        ? serviceProvider.GetService<IVodafoneApiClient>()
        : serviceProvider.GetService<IKinlyApiClient>();

    public SupplierConfiguration GetSupplierConfiguration() =>
        featureToggles.VodafoneIntegrationEnabled() ? vodafoneConfigOptions.Value : kinlyConfigOptions.Value;

}
