using System;
using VideoApi.Services.Clients.Models;
using VideoApi.Services.Clients.SupplierStub;
using VideoApi.Services.Clients.SupplierStub.Models;

namespace VideoApi.UnitTests.Services.Clients.SupplierStub;

public class SupplierStubApiClientMapperTests
{
    [Test]
    public void Should_map_book_hearing_request_to_supplier_stub_hearing_model()
    {
        // Arrange
        var request = new BookHearingRequest
        {
            VirtualCourtroomId = Guid.NewGuid(),
            TelephoneConferenceId = "telephone_conference_id"
        };

        // Act
        var result = request.ToSupplierStubHearingModel();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(request.VirtualCourtroomId.Value);
        result.Uris.PexipNode.Should().Be("pexip_node");
        result.Uris.Participant.Should().Be("participant");
        result.Uris.HearingRoomUri.Should().Be("hearing_room_uri");
        result.Uris.TelephoneConferenceId.Should().Be(request.TelephoneConferenceId);
        result.VirtualCourtroomId.Should().Be(request.VirtualCourtroomId);
        result.TelephoneConferenceId.Should().Be(request.TelephoneConferenceId);
    }
    
    [Test]
    public void Should_map_supplier_stub_hearing_model_to_book_hearing_response()
    {
        // Arrange
        var model = CreateHearing();

        // Act
        var result = model.ToBookHearingResponse();

        // Assert
        result.Should().NotBeNull();
        result.Uris.Should().Be(model.Uris);
        result.VirtualCourtroomId.Should().Be(model.VirtualCourtroomId);
    }
    
    [Test]
    public void Should_map_supplier_stub_hearing_model_to_retrieve_hearing_response()
    {
        // Arrange
        var model = CreateHearing();

        // Act
        var result = model.ToRetrieveHearingResponse();

        // Assert
        result.Should().NotBeNull();
        result.Uris.Should().Be(model.Uris);
        result.VirtualCourtroomId.Should().Be(model.VirtualCourtroomId);
        result.TelephoneConferenceId.Should().Be(model.TelephoneConferenceId);
    }

    private static Hearing CreateHearing()
    {
        return new Hearing
        {
            Id = Guid.NewGuid(),
            Uris = new MeetingUris
            {
                PexipNode = "pexip_node",
                Participant = "participant",
                HearingRoomUri = "hearing_room_uri",
                TelephoneConferenceId = "telephone_conference_id"
            },
            VirtualCourtroomId = Guid.NewGuid(),
            TelephoneConferenceId = "telephone_conference_id"
        };
    }
}
