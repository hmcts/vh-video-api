using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VideoApi.Common.Helpers;
using VideoApi.Services.Clients.Models;
using VideoApi.Services.Clients.SupplierStub.Models;
using VideoApi.Services.Clients.SupplierStub.Requests;

namespace VideoApi.Services.Clients.SupplierStub;

public interface ISupplierStubApiClient : ISupplierApiClient;

public class SupplierStubApiClient(HttpClient httpClient) : ISupplierStubApiClient
{
    public string BaseUrlAddress { get; set; }
    
    public async Task<BookHearingResponse> CreateHearingAsync(BookHearingRequest body)
    {
        var requestUri = GetRequestUrl("/hearings");
        var requestBody = body.ToSupplierStubHearingModel();
        var response = await httpClient.PostAsync(requestUri, CreateRequestBodyContent(requestBody));
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return ApiRequestHelper
            .DeserialiseForSupplier<Hearing>(response.Content.ReadAsStringAsync().Result)
            .ToBookHearingResponse();
    }

    public async Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(Guid hearingId, ConsultationRoomRequest body)
    {
        var hearing = await RetrieveHearing(hearingId);
        var newRoom = GenerateNewRoomForHearing(hearing, body.RoomLabelPrefix);

        await AddRoomToHearing(hearing, newRoom);

        return new CreateConsultationRoomResponse
        {
            RoomLabel = newRoom.Label
        };
    }

    public async Task<RetrieveHearingResponse> GetHearingAsync(Guid hearingId)
    {
        return (await RetrieveHearing(hearingId))?.ToRetrieveHearingResponse();
    }

    public async Task DeleteHearingAsync(Guid hearingId)
    {
        var requestUri = GetRequestUrl($"/hearings/{hearingId}");
        var response = await httpClient.DeleteAsync(requestUri);
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
    }

    public Task UpdateHearingAsync(Guid hearingId, UpdateHearingRequest body)
    {
        // We don't currently store or return the properties in this update request, so do nothing
        return Task.CompletedTask;
    }

    public Task<string> TransferParticipantAsync(Guid hearingId, TransferRequest body)
    {
        return Task.FromResult(body.To);
    }

    public Task<string> StartAsync(Guid hearingId, StartHearingRequest body)
    {
        return Task.FromResult(hearingId.ToString());
    }

    public Task<string> PauseHearingAsync(Guid hearingId)
    {
        return Task.FromResult(hearingId.ToString());
    }

    public Task<string> UpdateParticipantDisplayNameAsync(Guid hearingId, DisplayNameRequest body)
    {
        return Task.FromResult(hearingId.ToString());
    }

    public Task<string> EndHearingAsync(Guid hearingId)
    {
        return Task.FromResult(hearingId.ToString());
    }

    public Task<string> TechnicalAssistanceAsync(Guid hearingId)
    {
        return Task.FromResult(hearingId.ToString());
    }

    public Task<SelfTestParticipantResponse> RetrieveParticipantSelfTestScore(Guid participantId)
    {
        var response = new SelfTestParticipantResponse
        {
            Passed = true,
            Score = 1,
            UserId = participantId
        };
        
        return Task.FromResult(response);
    }

    public async Task<HealthCheckResponse> GetHealth()
    {
        var requestUri = GetRequestUrl("/hearings");
        var healthStatus = PlatformHealth.Healthy;
        
        try
        {
            await httpClient.GetAsync(requestUri);
        }
        catch (Exception)
        {
            healthStatus = PlatformHealth.Unhealthy;
        }

        return new HealthCheckResponse
        {
            HealthStatus = healthStatus
        };
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

    private async Task<Hearing> RetrieveHearing(Guid hearingId)
    {
        var requestUri = GetRequestUrl($"/hearings/{hearingId}");
        var response = await httpClient.GetAsync(requestUri);
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
        return ApiRequestHelper
            .DeserialiseForSupplier<Hearing>(response.Content.ReadAsStringAsync().Result);
    }

    private static Room GenerateNewRoomForHearing(Hearing hearing, string roomLabelPrefix)
    {
        var existingRoomsWithRequestedPrefix = hearing.Rooms?
            .Where(r => r.Label.StartsWith(roomLabelPrefix, StringComparison.CurrentCultureIgnoreCase))
            .ToList();

        var newRoom = new Room
        {
            Label = $"{roomLabelPrefix}{existingRoomsWithRequestedPrefix?.Count + 1}"
        };

        return newRoom;
    }

    private async Task AddRoomToHearing(Hearing hearing, Room room)
    {
        var requestUri = GetRequestUrl($"/hearings/{hearing.Id}");
        var updatedRooms = hearing.Rooms.ToList();
        updatedRooms.Add(room);
        var requestBody = new AddRoomRequest
        {
            Rooms = updatedRooms
        };
        var response = await httpClient.PatchAsync(requestUri, CreateRequestBodyContent(requestBody));
        EnsureSuccessStatusCodeOrThrowSupplierException(response);
    }
}
