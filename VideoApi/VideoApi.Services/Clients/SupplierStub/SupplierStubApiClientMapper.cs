using System;
using VideoApi.Services.Clients.Models;
using VideoApi.Services.Clients.SupplierStub.Models;

namespace VideoApi.Services.Clients.SupplierStub;

public static class SupplierStubApiClientMapper
{
    public static SupplierStubHearingModel ToSupplierStubHearingModel(this BookHearingRequest request)
    {
        return new SupplierStubHearingModel
        {
            Id = Guid.NewGuid(),
            Uris = new MeetingUris
            {
                PexipNode = "pexip_node",
                Participant = "participant",
                HearingRoomUri = "hearing_room_uri",
                TelephoneConferenceId = request.TelephoneConferenceId
            },
            VirtualCourtroomId = request.VirtualCourtroomId,
            TelephoneConferenceId = request.TelephoneConferenceId
        };
    }

    public static BookHearingResponse ToBookHearingResponse(this SupplierStubHearingModel model)
    {
        return new BookHearingResponse
        {
            Uris = model.Uris,
            VirtualCourtroomId = model.VirtualCourtroomId
        };
    }

    public static RetrieveHearingResponse ToRetrieveHearingResponse(this SupplierStubHearingModel model)
    {
        return new RetrieveHearingResponse
        {
            Uris = model.Uris,
            VirtualCourtroomId = model.VirtualCourtroomId,
            TelephoneConferenceId = model.TelephoneConferenceId
        };
    }
}
