using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VideoApi.Common.Helpers;
using VideoApi.Services.Clients.Models;
using VideoApi.Services.Clients.SupplierStub.Models;

namespace VideoApi.Services.Clients.SupplierStub;

public interface ISupplierStubApiClient : ISupplierApiClient;

public class SupplierStubApiClient(HttpClient httpClient) : ISupplierStubApiClient
{
    public string BaseUrlAddress { get; set; }
    
    public async Task<BookHearingResponse> CreateHearingAsync(BookHearingRequest body)
    {
        var requestUri = GetRequestUrl("/hearings");
        var mappedBody = body.ToSupplierStubHearingModel();
        var response = await httpClient.PostAsync(requestUri, CreateRequestBodyContent(mappedBody));
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return ApiRequestHelper
            .DeserialiseForSupplier<SupplierStubHearingModel>(response.Content.ReadAsStringAsync().Result)
            .ToBookHearingResponse();
    }

    public Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(Guid hearingId, ConsultationRoomRequest body)
    {
        throw new NotImplementedException();
    }

    public async Task<RetrieveHearingResponse> GetHearingAsync(Guid hearingId)
    {
        var requestUri = GetRequestUrl($"/hearings/{hearingId}");
        var response = await httpClient.GetAsync(requestUri);
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return ApiRequestHelper
            .DeserialiseForSupplier<SupplierStubHearingModel>(response.Content.ReadAsStringAsync().Result)
            .ToRetrieveHearingResponse();
    }

    public Task DeleteHearingAsync(Guid hearingId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateHearingAsync(Guid hearingId, UpdateHearingRequest body)
    {
        throw new NotImplementedException();
    }

    public Task<string> TransferParticipantAsync(Guid hearingId, TransferRequest body)
    {
        throw new NotImplementedException();
    }

    public Task<string> StartAsync(Guid hearingId, StartHearingRequest body)
    {
        throw new NotImplementedException();
    }

    public Task<string> PauseHearingAsync(Guid hearingId)
    {
        throw new NotImplementedException();
    }

    public Task<string> UpdateParticipantDisplayNameAsync(Guid hearingId, DisplayNameRequest body)
    {
        throw new NotImplementedException();
    }

    public Task<string> EndHearingAsync(Guid hearingId)
    {
        throw new NotImplementedException();
    }

    public Task<string> TechnicalAssistanceAsync(Guid hearingId)
    {
        throw new NotImplementedException();
    }

    public Task<SelfTestParticipantResponse> RetrieveParticipantSelfTestScore(Guid participantId)
    {
        throw new NotImplementedException();
    }

    public Task<HealthCheckResponse> GetHealth()
    {
        throw new NotImplementedException();
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
    
    private static HttpContent CreateRequestBodyContent<T>(T request)
    {
        return new StringContent(ApiRequestHelper.SerialiseForSupplier(request), Encoding.UTF8, "application/json");
    }
}
