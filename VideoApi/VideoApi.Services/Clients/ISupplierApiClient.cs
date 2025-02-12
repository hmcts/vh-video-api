using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VideoApi.Common.Helpers;
using VideoApi.Services.Clients.Models;

namespace VideoApi.Services.Clients;

public interface ISupplierApiClient
{
    // hearing operations
    Task<BookHearingResponse> CreateHearingAsync(BookHearingRequest body);
    Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(Guid hearingId, ConsultationRoomRequest body);
    Task<RetrieveHearingResponse> GetHearingAsync(Guid hearingId);
    Task DeleteHearingAsync(Guid hearingId);
    Task UpdateHearingAsync(Guid hearingId, UpdateHearingRequest body);
    
    // conference operations
    Task<string> TransferParticipantAsync(Guid hearingId, TransferRequest body);
    Task<string> StartAsync(Guid hearingId, StartHearingRequest body);
    Task<string> PauseHearingAsync(Guid hearingId);
    Task<string> UpdateParticipantDisplayNameAsync(Guid hearingId, DisplayNameRequest body);
    Task<string> EndHearingAsync(Guid hearingId);
    Task<string> TechnicalAssistanceAsync(Guid hearingId);
    
    // self-test operations
    Task<SelfTestParticipantResponse> RetrieveParticipantSelfTestScore(Guid participantId);
    
    // health operations
    Task<HealthCheckResponse> GetHealth();
}


/// <summary>
/// C# implementation of the Supplier API client https://si.dev.vh-hmcts.co.uk/v3/api-docs
/// </summary>
/// <param name="httpClient"></param>
[ExcludeFromCodeCoverage]
public class SupplierApiClient(HttpClient httpClient) : IVodafoneApiClient
{
    public string BaseUrlAddress { get; set; }
    public async Task<BookHearingResponse> CreateHearingAsync(BookHearingRequest body)
    {
        var requestUri = GetRequestUrl("/hearing");
        var response = await httpClient.PostAsync(requestUri, RequestBody.Set(body));
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return ApiRequestHelper.Deserialise<BookHearingResponse>(response.Content.ReadAsStringAsync().Result);
    }

    public async Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(Guid hearingId,
        ConsultationRoomRequest body)
    {
        var requestUri = GetRequestUrl($"/hearing/{hearingId}/consultation-room");
        var response = await httpClient.PostAsync(requestUri, RequestBody.Set(body));
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return ApiRequestHelper.Deserialise<CreateConsultationRoomResponse>(response.Content.ReadAsStringAsync()
            .Result);
    }

    public async Task<RetrieveHearingResponse> GetHearingAsync(Guid hearingId)
    {
        var requestUri = GetRequestUrl($"/hearing/{hearingId}");
        var response = await httpClient.GetAsync(requestUri);
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return ApiRequestHelper.Deserialise<RetrieveHearingResponse>(response.Content.ReadAsStringAsync().Result);
    }

    public async Task DeleteHearingAsync(Guid hearingId)
    {
        var requestUri = GetRequestUrl($"/hearing/{hearingId}");
        var response = await httpClient.DeleteAsync(requestUri);
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
    }

    public async Task UpdateHearingAsync(Guid hearingId, UpdateHearingRequest body)
    {
        var requestUrl = GetRequestUrl($"/hearing/{hearingId}");
        var response = await httpClient.PatchAsync(requestUrl, RequestBody.Set(body));
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
    }

    public async Task<string> TransferParticipantAsync(Guid hearingId, TransferRequest body)
    {
        var requestUrl = GetRequestUrl($"/hearing/{hearingId}/transfer");
        var response = await httpClient.PostAsync(requestUrl, RequestBody.Set(body));
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> StartAsync(Guid hearingId, StartHearingRequest body)
    {
        var requestUrl = GetRequestUrl($"/hearing/{hearingId}/start");
        var response = await httpClient.PostAsync(requestUrl, RequestBody.Set(body));
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PauseHearingAsync(Guid hearingId)
    {
        var requestUrl = GetRequestUrl($"/hearing/{hearingId}/pause");
        var response = await httpClient.PostAsync(requestUrl, null);
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> UpdateParticipantDisplayNameAsync(Guid hearingId, DisplayNameRequest body)
    {
        var requestUrl = GetRequestUrl($"/hearing/{hearingId}/participant-name");
        var response = await httpClient.PostAsync(requestUrl, RequestBody.Set(body));
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> EndHearingAsync(Guid hearingId)
    {
        var requestUrl = GetRequestUrl($"/hearing/{hearingId}/end");
        var response = await httpClient.PostAsync(requestUrl, null);
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> TechnicalAssistanceAsync(Guid hearingId)
    {
        var requestUrl = GetRequestUrl($"/hearing/{hearingId}/assistance");
        var response = await httpClient.PostAsync(requestUrl, null);
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<SelfTestParticipantResponse> RetrieveParticipantSelfTestScore(Guid participantId)
    {
        var requestUrl = GetRequestUrl($"/selfTest/testCall/{participantId}");
        var response = await httpClient.GetAsync(requestUrl);
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return ApiRequestHelper.Deserialise<SelfTestParticipantResponse>(response.Content.ReadAsStringAsync().Result);
    }

    public async Task<HealthCheckResponse> GetHealth()
    {
        var requestUrl = GetRequestUrl("/health");
        var response = await httpClient.GetAsync(requestUrl);
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return ApiRequestHelper.Deserialise<HealthCheckResponse>(response.Content.ReadAsStringAsync().Result);
    }
    
    private static void EnsureSuccessStatusCodeOrThrowSupplierException(HttpResponseMessage response)
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
    
    private string GetRequestUrl(string requestUri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestUri);
        return $"{BaseUrlAddress.TrimEnd('/')}{requestUri}";
    }
}

public class SupplierApiException(HttpStatusCode? statusCode, string response, Exception innerException)
    : Exception("There was a problem returned by the Supplier API", innerException)
{
    public HttpStatusCode StatusCode { get; } = (HttpStatusCode)statusCode!;
    public string Response { get; } = response;
}

