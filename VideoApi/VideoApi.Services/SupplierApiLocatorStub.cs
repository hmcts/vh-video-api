using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Services.Clients;

namespace VideoApi.Services;

[ExcludeFromCodeCoverage]
public class SupplierApiSelectorStub : ISupplierApiSelector
{
    private readonly IOptions<KinlyConfiguration> _kinlyConfigOptions;

    public SupplierApiSelectorStub(IOptions<KinlyConfiguration> kinlyConfigOptions)
    {
        _kinlyConfigOptions = kinlyConfigOptions;
    }
    public ISupplierApiClient GetHttpClient()
    {
        throw new NotImplementedException();
    }

    public SupplierConfiguration GetSupplierConfiguration() => _kinlyConfigOptions.Value;
}
