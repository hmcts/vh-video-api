using System;
using System.Net.Http;
using System.Text;
using VideoApi.Common.Helpers;

namespace VideoApi.Services.Clients;

public abstract class SupplierApiClientBase
{
    public string BaseUrlAddress { get; set; }
    
    protected static void EnsureSuccessStatusCodeOrThrowSupplierException(HttpResponseMessage response)
    {
        try
        {
            response.EnsureSuccessStatusCode();
        } catch (HttpRequestException e)
        {
            var responseText = response.Content.ReadAsStringAsync().Result;
            throw new SupplierApiException(e.StatusCode, responseText, e);
        }
    }
    
    protected string GetRequestUrl(string requestUri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestUri);
        return $"{BaseUrlAddress.TrimEnd('/')}{requestUri}";
    }
    
    protected static HttpContent CreateRequestBodyContent<T>(T request)
    {
        return new StringContent(ApiRequestHelper.SerialiseForSupplier(request), Encoding.UTF8, "application/json");
    }
}
