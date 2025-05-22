using System;
using System.Threading.Tasks;
using VideoApi.Services.Clients.Models;

namespace VideoApi.Services.Clients.SupplierStub;

public interface ISupplierStubClient : ISupplierClient;

public class SupplierStubClient : ISupplierStubClient
{
    private readonly Random _random = new();

    public Task<BookHearingResponse> CreateHearingAsync(BookHearingRequest body)
    {
        return Task.FromResult(new BookHearingResponse
        {
            Uris = new MeetingUris
            {
                PexipNode = "pexip_node",
                Participant = "participant",
                HearingRoomUri = "hearing_room_uri",
                TelephoneConferenceId = body.TelephoneConferenceId
            },
            VirtualCourtroomId = body.VirtualCourtroomId
        });
    }

    public Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(Guid hearingId, ConsultationRoomRequest body)
    {
        var roomNumber = _random.Next(1, 10);
        return Task.FromResult(new CreateConsultationRoomResponse
        {
            RoomLabel = $"{body.RoomLabelPrefix}{roomNumber}"
        });
    }

    public Task<RetrieveHearingResponse> GetHearingAsync(Guid hearingId)
    {
        var testHearing = new RetrieveHearingResponse
        {
            Uris = new MeetingUris
            {
                PexipNode = "pexip_node",
                Participant = "participant",
                HearingRoomUri = "hearing_room_uri",
                TelephoneConferenceId = hearingId.ToString()
            },
            VirtualCourtroomId = hearingId,
            TelephoneConferenceId = hearingId.ToString()
        };
        return Task.FromResult(testHearing);
    }

    public Task DeleteHearingAsync(Guid hearingId)
    {
        return Task.CompletedTask;
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

    public Task<HealthCheckResponse> GetHealth()
    {
        return Task.FromResult(new HealthCheckResponse
        {
            HealthStatus = PlatformHealth.Healthy
        });
    }
}
