using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using VideoApi.Services.Clients;
using VideoApi.Services.Clients.Models;

namespace VideoApi.UnitTests.Services.Clients;

public class SupplierApiClientTests
{
    [Test]
    public async Task should_map_conference_details_on_booking()
    {
        // arrange
        var conferenceId = Guid.Parse("b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d");
        var baseAddress = "http://supplier-api";
        
        var responseBody = """
                           {
                             "uris": {
                               "pexip_node": "test.unit.vh.co.uk",
                               "participant": "hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_WAITING_ROOM",
                               "hearing_room_uri": "hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_HEARING_ROOM"
                             },
                             "virtual_courtroom_id": "b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d"
                           }
                           """;
        
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Post, $"{baseAddress}/hearing")
            .Respond(HttpStatusCode.OK, "application/json", responseBody);
        var httpClient = mockHttp.ToHttpClient();
        var supplierApiClient = new SupplierApiClient(httpClient)
        {
            BaseUrlAddress = baseAddress
        };

        // act
        var result = await supplierApiClient.CreateHearingAsync(new BookHearingRequest()
        {
            VirtualCourtroomId = conferenceId,
            CallbackUri = "https://vh-video-web.dev.platform.hmcts.net/callback",
            RecordingEnabled = false,
            RecordingUrl =
                "rtmps://vh-recorder:443/vh-recording-app/ABA4-AutomationTestHearingbac29d98326e4d118783bbdf9f1c24f6-77451c22-85cf-4b0f-90f7-146d09472843",
            StreamingEnabled = false,
            TelephoneConferenceId = "75926535",
            RoomType = "VMR",
            AudioPlaybackLanguage = "English",
            JvsEndpoint = []
        });
        
        Assert.That(result.VirtualCourtroomId, Is.EqualTo(conferenceId));
        Assert.That(result.Uris.PexipNode, Is.EqualTo("test.unit.vh.co.uk"));
        Assert.That(result.Uris.Participant, Is.EqualTo("hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_WAITING_ROOM"));
        Assert.That(result.Uris.HearingRoomUri, Is.EqualTo("hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_HEARING_ROOM"));
    }

    [Test]
    public async Task should_map_conference_on_retrieve_hearing_details()
    {
        // arrange
        var conferenceId = Guid.Parse("b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d");
        var baseAddress = "http://supplier-api";
        
        var responseBody = """
                           {
                             "uris": {
                               "pexip_node": "test.unit.vh.co.uk",
                               "participant": "hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_WAITING_ROOM",
                               "hearing_room_uri": "hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_HEARING_ROOM"
                             },
                             "virtual_courtroom_id": "b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d",
                             "consultation_rooms":[]
                           }
                           """;
        
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, $"{baseAddress}/hearing/{conferenceId}")
            .Respond(HttpStatusCode.OK, "application/json", responseBody);
        var httpClient = mockHttp.ToHttpClient();
        var supplierApiClient = new SupplierApiClient(httpClient)
        {
            BaseUrlAddress = baseAddress
        };

        // act
        var result = await supplierApiClient.GetHearingAsync(conferenceId);
        
        Assert.That(result.VirtualCourtroomId, Is.EqualTo(conferenceId));
        Assert.That(result.Uris.PexipNode, Is.EqualTo("test.unit.vh.co.uk"));
        Assert.That(result.Uris.Participant, Is.EqualTo("hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_WAITING_ROOM"));
        Assert.That(result.Uris.HearingRoomUri, Is.EqualTo("hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_HEARING_ROOM"));
    }
}
