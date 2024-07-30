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

// TODO deprecate this class, call GetHttpClient and GetSupplierConfiguration from SupplierPlatformService instead
public class SupplierApiSelector : ISupplierApiSelector
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IFeatureToggles _featureToggles;
    private readonly IOptions<KinlyConfiguration> _kinlyConfigOptions;
    private readonly IOptions<VodafoneConfiguration> _vodafoneConfigOptions;

    public SupplierApiSelector(IServiceProvider serviceProvider,
        IFeatureToggles featureToggles,
        IOptions<KinlyConfiguration> kinlyConfigOptions,
        IOptions<VodafoneConfiguration> vodafoneConfigOptions)
    {
        _serviceProvider = serviceProvider;
        _featureToggles = featureToggles;
        _kinlyConfigOptions = kinlyConfigOptions;
        _vodafoneConfigOptions = vodafoneConfigOptions;
    }

    public ISupplierApiClient GetHttpClient() => _featureToggles.VodafoneIntegrationEnabled()
        ? _serviceProvider.GetService<IVodafoneApiClient>()
        : _serviceProvider.GetService<IKinlyApiClient>();

    public SupplierConfiguration GetSupplierConfiguration() =>
        _featureToggles.VodafoneIntegrationEnabled() ? _vodafoneConfigOptions.Value : _kinlyConfigOptions.Value;

}
